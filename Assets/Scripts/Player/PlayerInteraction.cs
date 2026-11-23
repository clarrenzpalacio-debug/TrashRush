using System;
using System.Collections.Generic;
using UnityEngine;
using TrashRush.Trash;
using TrashRush.Recycling;
using TrashRush.Core;
using TrashRush.Audio;
using TrashRush.Inventory;

namespace TrashRush.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float interactRange = 2.4f;
        [SerializeField] private Transform carryPoint;
        [SerializeField] private LayerMask interactableMask = ~0;

        [Header("State")]
        private TrashItem currentTargetTrash = null;
        private RecyclingBin currentTargetBin = null;
        private RecyclingBin[] allBinsInScene = null;
        private string feedbackMessage = "";
        private float feedbackTimer = 0f;

        // ── Inventory ───────────────────────────────────────────────────
        private TrashBagInventory trashBag;

        // ── Legacy-compatible properties ────────────────────────────────
        // CarriedItem now returns null (multi-item bag replaces single carry).
        // IsCarrying returns true when the bag has at least one item.
        public TrashItem CarriedItem => null;
        public bool IsCarrying => trashBag != null && trashBag.ItemCount > 0;
        public RecyclingBin CurrentTargetBin => currentTargetBin;
        public string FeedbackMessage => feedbackMessage;

        /// <summary>Access the player's trash bag inventory.</summary>
        public TrashBagInventory TrashBag => trashBag;

        public event Action<TrashItem> OnItemPickedUp;
        public event Action<bool, TrashItem, RecyclingBin> OnItemDeposited;

        private void Awake()
        {
            if (carryPoint == null)
            {
                Transform cp = transform.Find("CarryPoint");
                if (cp != null) carryPoint = cp;
            }

            // Attach or find the TrashBagInventory component
            trashBag = GetComponent<TrashBagInventory>();
            if (trashBag == null)
            {
                trashBag = gameObject.AddComponent<TrashBagInventory>();
            }
        }

        private void Start()
        {
            RefreshBinsCache();
        }

        public void RefreshBinsCache()
        {
            allBinsInScene = FindObjectsOfType<RecyclingBin>();
        }

        private void UpdateBinHighlights()
        {
            if (allBinsInScene == null || allBinsInScene.Length == 0)
            {
                RefreshBinsCache();
            }

            if (allBinsInScene != null)
            {
                for (int i = 0; i < allBinsInScene.Length; i++)
                {
                    if (allBinsInScene[i] != null)
                    {
                        // Highlight bins whose category matches ANY item in the bag
                        bool shouldHighlight = trashBag != null
                            && trashBag.ItemCount > 0
                            && trashBag.HasCategory(allBinsInScene[i].AcceptedCategory);
                        allBinsInScene[i].SetHighlight(shouldHighlight);
                    }
                }
            }
        }

        private void Update()
        {
            DetectInteractables();

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                Interact();
            }

            if (feedbackTimer > 0f)
            {
                feedbackTimer -= Time.deltaTime;
                if (feedbackTimer <= 0f) feedbackMessage = "";
            }
        }

        private void DetectInteractables()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableMask);

            TrashItem closestTrash = null;
            RecyclingBin closestBin = null;
            float closestTrashDist = interactRange;
            float closestBinDist = interactRange;

            foreach (var hit in hits)
            {
                // Look for trash to pick up when bag is NOT full
                if (trashBag == null || !trashBag.IsFull)
                {
                    TrashItem trash = hit.GetComponentInParent<TrashItem>();
                    if (trash != null && !trash.IsCollected)
                    {
                        float d = Vector3.Distance(transform.position, trash.transform.position);
                        if (d < closestTrashDist)
                        {
                            closestTrashDist = d;
                            closestTrash = trash;
                        }
                    }
                }

                // Always check for recycling bins
                RecyclingBin bin = hit.GetComponentInParent<RecyclingBin>();
                if (bin != null)
                {
                    float d = Vector3.Distance(transform.position, bin.transform.position);
                    if (d < closestBinDist)
                    {
                        closestBinDist = d;
                        closestBin = bin;
                    }
                }
            }

            if (currentTargetBin != null && currentTargetBin != closestBin)
            {
                currentTargetBin.SetPlayerNearby(false);
            }
            if (closestBin != null)
            {
                closestBin.SetPlayerNearby(true);
            }

            currentTargetTrash = closestTrash;
            currentTargetBin = closestBin;
        }

        private void Interact()
        {
            // Priority 1: If near a bin AND carrying items, try to deposit
            if (currentTargetBin != null && trashBag != null && trashBag.ItemCount > 0)
            {
                DepositTrash(currentTargetBin);
                return;
            }

            // Priority 2: Pick up nearby trash
            if (currentTargetTrash != null)
            {
                PickupTrash(currentTargetTrash);
            }
        }

        private void PickupTrash(TrashItem trash)
        {
            if (trashBag == null) return;

            if (trashBag.IsFull)
            {
                ShowFeedback("BAG IS FULL! Deposit items at a recycling bin first.", 2.5f);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.SFXType.IncorrectSort);
                }
                return;
            }

            bool added = trashBag.TryAddItem(trash);
            if (!added) return; // duplicate or other failure

            UpdateBinHighlights();

            ShowFeedback($"Collected: {trash.ItemName} ({trash.Category})  — Bag: {trashBag.ItemCount}/{trashBag.Capacity}", 2.5f);
            OnItemPickedUp?.Invoke(trash);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.SFXType.Pickup);
            }
        }

        private void DepositTrash(RecyclingBin bin)
        {
            if (trashBag == null || trashBag.ItemCount == 0) return;

            TrashCategory binCat = bin.AcceptedCategory;

            // Pull out only items matching this bin's category
            List<TrashItem> matchingItems = trashBag.RemoveItemsByCategory(binCat);

            if (matchingItems.Count > 0)
            {
                // All these items are correct — score each
                int totalPoints = 0;
                foreach (var item in matchingItems)
                {
                    bool isCorrect = true; // category matched
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.ProcessRecycle(isCorrect, item, binCat);
                    }
                    OnItemDeposited?.Invoke(isCorrect, item, bin);
                    totalPoints += item.PointValue;

                    // Destroy the item
                    item.OnDeposited();
                }

                ShowFeedback(
                    $"CORRECT! +{totalPoints} Points ({matchingItems.Count} {binCat} item{(matchingItems.Count > 1 ? "s" : "")}) — Bag: {trashBag.ItemCount}/{trashBag.Capacity}",
                    2.5f);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.SFXType.CorrectSort);
                }
            }
            else
            {
                // No matching items — wrong bin! Apply penalty for each item? No: show warning only.
                // The player has items but none match this bin category.
                ShowFeedback(
                    $"WRONG BIN! You have no {binCat} items. Check your bag contents!",
                    2.5f);

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(AudioManager.SFXType.IncorrectSort);
                }
            }

            UpdateBinHighlights();
        }

        public void ShowFeedback(string message, float duration)
        {
            feedbackMessage = message;
            feedbackTimer = duration;
        }

        public string GetCurrentPrompt()
        {
            bool hasBag = trashBag != null;
            bool bagHasItems = hasBag && trashBag.ItemCount > 0;
            bool bagIsFull = hasBag && trashBag.IsFull;

            // Near a bin and carrying items — show deposit prompt
            if (currentTargetBin != null && bagHasItems)
            {
                TrashCategory binCat = currentTargetBin.AcceptedCategory;
                int matchCount = hasBag ? trashBag.CountByCategory(binCat) : 0;
                string catUpper = binCat.ToString().ToUpper();

                if (matchCount > 0)
                {
                    return $"[E] Deposit {matchCount} {catUpper} item{(matchCount > 1 ? "s" : "")}  <color=#00FF66>[MATCHING]</color>";
                }
                else
                {
                    return $"[E] {catUpper} Bin  <color=#FF4444>[NO MATCHING ITEMS]</color>";
                }
            }

            // Near trash — show pickup prompt
            if (currentTargetTrash != null && !bagIsFull)
            {
                return $"[E] Pick up: {currentTargetTrash.ItemName} <color=yellow>[{currentTargetTrash.Category}]</color>";
            }

            // Bag is full, near trash
            if (currentTargetTrash != null && bagIsFull)
            {
                return "<color=#FF4444>BAG FULL!</color> Deposit items at a recycling bin.";
            }

            // Carrying items, not near anything special
            if (bagHasItems)
            {
                return $"Bag: {trashBag.GetContentsSummary()} — Find a matching recycling bin!";
            }

            return "";
        }
    }
}
