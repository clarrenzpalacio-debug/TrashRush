using System;
using UnityEngine;

namespace TrashRush.Player
{
    /// <summary>
    /// Manages player stamina for sprinting. Attach to the same GameObject as PlayerController.
    /// Stamina drains while sprinting and regenerates after a configurable delay.
    /// Capacity values are exposed for future upgrade integration.
    /// </summary>
    public class PlayerStamina : MonoBehaviour
    {
        [Header("Stamina Settings")]
        [SerializeField] private float maxStamina = 100f;
        [SerializeField] private float sprintDrainRate = 20f;
        [SerializeField] private float regenRate = 15f;
        [SerializeField] private float regenDelay = 1.5f;

        // Runtime state
        private float currentStamina;
        private float regenDelayTimer;
        private bool isSprinting;

        // ── Public API ──────────────────────────────────────────────────

        /// <summary>Current stamina value (0 to MaxStamina).</summary>
        public float CurrentStamina => currentStamina;

        /// <summary>Maximum stamina capacity.</summary>
        public float MaxStamina => maxStamina;

        /// <summary>Stamina as a 0–1 fraction (for UI fill).</summary>
        public float StaminaRatio => maxStamina > 0f ? currentStamina / maxStamina : 0f;

        /// <summary>True when the player has stamina available for sprinting.</summary>
        public bool CanSprint => currentStamina > 0f;

        /// <summary>True when stamina is currently being drained by sprinting.</summary>
        public bool IsDraining => isSprinting;

        /// <summary>Fired whenever stamina changes: (current, max).</summary>
        public event Action<float, float> OnStaminaChanged;

        // ── Lifecycle ───────────────────────────────────────────────────

        private void Awake()
        {
            currentStamina = maxStamina;
            regenDelayTimer = 0f;
        }

        // ── Called by PlayerController each frame ───────────────────────

        /// <summary>
        /// Notify the stamina system whether the player is actively sprinting this frame.
        /// Call this from PlayerController.Update() BEFORE reading CanSprint.
        /// </summary>
        public void SetSprinting(bool sprinting)
        {
            isSprinting = sprinting;
        }

        /// <summary>
        /// Tick stamina drain and regeneration. Call once per frame from PlayerController.
        /// </summary>
        public void Tick()
        {
            if (isSprinting)
            {
                // Drain stamina
                currentStamina -= sprintDrainRate * Time.deltaTime;
                currentStamina = Mathf.Max(0f, currentStamina);
                regenDelayTimer = regenDelay; // reset regen delay

                OnStaminaChanged?.Invoke(currentStamina, maxStamina);
            }
            else
            {
                // Count down regen delay
                if (regenDelayTimer > 0f)
                {
                    regenDelayTimer -= Time.deltaTime;
                }

                // Regenerate after delay
                if (regenDelayTimer <= 0f && currentStamina < maxStamina)
                {
                    currentStamina += regenRate * Time.deltaTime;
                    currentStamina = Mathf.Min(currentStamina, maxStamina);

                    OnStaminaChanged?.Invoke(currentStamina, maxStamina);
                }
            }
        }

        // ── Upgrade API (for future shop/progression) ───────────────────

        /// <summary>
        /// Set a new maximum stamina. Current stamina is clamped to the new max.
        /// </summary>
        public void SetMaxStamina(float newMax)
        {
            maxStamina = Mathf.Max(1f, newMax);
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }

        /// <summary>
        /// Fully restore stamina to max.
        /// </summary>
        public void RestoreFull()
        {
            currentStamina = maxStamina;
            regenDelayTimer = 0f;
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }
}
