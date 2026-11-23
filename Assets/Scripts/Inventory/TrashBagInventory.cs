using System.Collections.Generic;
using System;
using UnityEngine;
using TrashRush.Core;
using TrashRush.Trash;

namespace TrashRush.Inventory
{
    /// <summary>
    /// Manages the player's trash bag — a multi-item inventory that stores collected
    /// TrashItems until they are deposited into recycling bins.
    /// Capacity is upgrade-ready: set DefaultCapacity or call SetCapacity() at runtime.
    /// </summary>
    public class TrashBagInventory : MonoBehaviour
    {
        [Header("Capacity Settings")]
        [SerializeField] private int defaultCapacity = 10;

        /// <summary>Current maximum number of items the bag can hold.</summary>
        public int Capacity { get; private set; }

        /// <summary>Number of items currently in the bag.</summary>
        public int ItemCount => items.Count;

        /// <summary>True when the bag cannot accept more items.</summary>
        public bool IsFull => items.Count >= Capacity;

        // The authoritative item list. Items are kept alive (hidden) until deposited.
        private readonly List<TrashItem> items = new List<TrashItem>();

        // Fast duplicate guard — prevents the same GameObject from being added twice.
        private readonly HashSet<int> itemInstanceIds = new HashSet<int>();

        // ── Events ──────────────────────────────────────────────────────────
        /// <summary>Fired after an item is successfully added.</summary>
        public event Action<TrashItem> OnItemAdded;

        /// <summary>Fired after one or more items are deposited (correct, list of deposited items).</summary>
        public event Action<bool, List<TrashItem>> OnItemsDeposited;

        /// <summary>Fired when a pickup is rejected because the bag is full.</summary>
        public event Action OnBagFull;

        /// <summary>Fired whenever the item count changes (new count, capacity).</summary>
        public event Action<int, int> OnCountChanged;

        // ── Lifecycle ───────────────────────────────────────────────────────
        private void Awake()
        {
            Capacity = defaultCapacity;
        }

        // ── Public API ──────────────────────────────────────────────────────

        /// <summary>
        /// Try to add a TrashItem to the bag.
        /// Returns true on success. Returns false if the bag is full or if the
        /// item is a duplicate (already in the bag).
        /// </summary>
        public bool TryAddItem(TrashItem item)
        {
            if (item == null) return false;

            // Duplicate guard
            int id = item.gameObject.GetInstanceID();
            if (itemInstanceIds.Contains(id))
            {
                Debug.LogWarning($"[TrashBag] Duplicate pickup blocked: {item.ItemName}");
                return false;
            }

            // Capacity check
            if (IsFull)
            {
                OnBagFull?.Invoke();
                return false;
            }

            items.Add(item);
            itemInstanceIds.Add(id);

            // Hide the item in the world (keep it alive for identity tracking)
            item.OnStoredInBag();

            OnItemAdded?.Invoke(item);
            OnCountChanged?.Invoke(items.Count, Capacity);
            return true;
        }

        /// <summary>
        /// Deposit all items matching the given category.
        /// Returns the list of deposited items (empty list if none matched).
        /// Does NOT destroy the items — the caller (PlayerInteraction) handles
        /// scoring and then calls TrashItem.OnDeposited().
        /// </summary>
        public List<TrashItem> RemoveItemsByCategory(TrashCategory category)
        {
            List<TrashItem> removed = new List<TrashItem>();

            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] != null && items[i].Category == category)
                {
                    removed.Add(items[i]);
                    itemInstanceIds.Remove(items[i].gameObject.GetInstanceID());
                    items.RemoveAt(i);
                }
            }

            if (removed.Count > 0)
            {
                OnCountChanged?.Invoke(items.Count, Capacity);
            }

            return removed;
        }

        /// <summary>
        /// Deposit ALL items currently in the bag regardless of category.
        /// Used when the player deposits into the wrong bin — each item is scored individually.
        /// Returns every item that was in the bag.
        /// </summary>
        public List<TrashItem> RemoveAllItems()
        {
            List<TrashItem> removed = new List<TrashItem>(items);
            items.Clear();
            itemInstanceIds.Clear();
            OnCountChanged?.Invoke(0, Capacity);
            return removed;
        }

        /// <summary>
        /// Returns true if the bag contains at least one item of the given category.
        /// </summary>
        public bool HasCategory(TrashCategory category)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].Category == category)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Count how many items of a specific category are in the bag.
        /// </summary>
        public int CountByCategory(TrashCategory category)
        {
            int count = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].Category == category)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Returns a read-only snapshot of all items currently in the bag.
        /// </summary>
        public IReadOnlyList<TrashItem> GetItems()
        {
            return items.AsReadOnly();
        }

        /// <summary>
        /// Returns a summary string of the bag contents grouped by category.
        /// Example: "Paper x3, Plastic x1"
        /// </summary>
        public string GetContentsSummary()
        {
            if (items.Count == 0) return "Empty";

            // Count per category
            Dictionary<TrashCategory, int> counts = new Dictionary<TrashCategory, int>();
            foreach (var item in items)
            {
                if (item == null) continue;
                if (counts.ContainsKey(item.Category))
                    counts[item.Category]++;
                else
                    counts[item.Category] = 1;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool first = true;
            foreach (var kvp in counts)
            {
                if (!first) sb.Append(", ");
                sb.Append($"{kvp.Key} x{kvp.Value}");
                first = false;
            }
            return sb.ToString();
        }

        // ── Upgrade API ─────────────────────────────────────────────────────

        /// <summary>
        /// Set a new capacity. Used by upgrade/shop systems.
        /// </summary>
        public void SetCapacity(int newCapacity)
        {
            Capacity = Mathf.Max(1, newCapacity);
            OnCountChanged?.Invoke(items.Count, Capacity);
        }

        /// <summary>
        /// Convenience: returns the upgrade level based on capacity.
        /// Level 1 = 10, Level 2 = 15, Level 3 = 20, Level 4 = 25.
        /// Returns 0 for unrecognized capacities (custom).
        /// </summary>
        public int GetUpgradeLevel()
        {
            switch (Capacity)
            {
                case 10: return 1;
                case 15: return 2;
                case 20: return 3;
                case 25: return 4;
                default: return 0;
            }
        }
    }
}
