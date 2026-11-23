using System;
using UnityEngine;
using TrashRush.Core;
using TrashRush.Trash;

namespace TrashRush.Recycling
{
    public class RecyclingBin : MonoBehaviour
    {
        [Header("Bin Configuration")]
        [SerializeField] private TrashCategory acceptedCategory;
        [SerializeField] private string categoryDisplayName = "Recycling Bin";
        [SerializeField] private Transform lidTransform;

        [Header("Visual Elements")]
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private Transform floatingLabelTransform;
        [SerializeField] private Light highlightLight;

        [Header("Audio & FX")]
        [SerializeField] private ParticleSystem correctEffect;
        [SerializeField] private ParticleSystem incorrectEffect;

        public TrashCategory AcceptedCategory => acceptedCategory;
        public string CategoryDisplayName => categoryDisplayName;

        private float lidOpenProgress = 0f;
        private bool isPlayerNearby = false;
        private bool isHighlighted = false;

        private Color normalColor;
        private Material bodyMatInstance;
        private Vector3 initialFloatingLabelPos;

        public event Action<bool, TrashItem> OnTrashDeposited;

        public void SetCategory(TrashCategory category, string displayName)
        {
            acceptedCategory = category;
            categoryDisplayName = displayName;
        }

        public void Configure(TrashCategory category, Renderer renderer)
        {
            acceptedCategory = category;
            categoryDisplayName = category.ToString();
            bodyRenderer = renderer;
        }

        public void SetFloatingLabelTransform(Transform flTransform)
        {
            floatingLabelTransform = flTransform;
        }

        public void SetHighlightLight(Light light)
        {
            highlightLight = light;
        }

        private void Awake()
        {
            if (bodyRenderer == null)
            {
                Transform bodyTr = transform.Find("BinBody");
                if (bodyTr != null) bodyRenderer = bodyTr.GetComponent<Renderer>();
            }

            if (bodyRenderer != null)
            {
                bodyMatInstance = bodyRenderer.material; // instantiate for individual emission/color pulsing
                normalColor = bodyMatInstance.color;
            }

            if (floatingLabelTransform == null)
            {
                floatingLabelTransform = transform.Find("FloatingIconBillboard");
            }

            if (floatingLabelTransform != null)
            {
                initialFloatingLabelPos = floatingLabelTransform.localPosition;
            }

            if (highlightLight == null)
            {
                Transform lt = transform.Find("HighlightLight");
                if (lt != null) highlightLight = lt.GetComponent<Light>();
            }
        }

        private void Update()
        {
            // Smoothly animate lid opening when player is near
            float targetProgress = isPlayerNearby ? 1f : 0f;
            lidOpenProgress = Mathf.MoveTowards(lidOpenProgress, targetProgress, Time.deltaTime * 4f);
            if (lidTransform != null)
            {
                lidTransform.localRotation = Quaternion.Euler(-30f * lidOpenProgress, 0f, 0f);
            }

            // Animate Floating Label: gentle bobbing and face main camera
            if (floatingLabelTransform != null)
            {
                float floatBob = Mathf.Sin(Time.time * 2.5f) * 0.08f;
                floatingLabelTransform.localPosition = initialFloatingLabelPos + new Vector3(0, floatBob, 0);

                if (Camera.main != null)
                {
                    // Face camera (billboard)
                    Vector3 lookDir = floatingLabelTransform.position - Camera.main.transform.position;
                    if (lookDir.sqrMagnitude > 0.001f)
                    {
                        floatingLabelTransform.rotation = Quaternion.LookRotation(lookDir);
                    }
                }
            }

            // Highlight pulsation when player is carrying matching trash
            if (isHighlighted)
            {
                float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 6f);
                if (highlightLight != null)
                {
                    highlightLight.enabled = true;
                    highlightLight.intensity = Mathf.Lerp(1.2f, 3.2f, pulse);
                }

                if (bodyMatInstance != null)
                {
                    bodyMatInstance.EnableKeyword("_EMISSION");
                    Color emissionColor = normalColor * (0.35f + pulse * 0.65f);
                    bodyMatInstance.SetColor("_EmissionColor", emissionColor);
                }

                // Pulsate scale of floating label slightly
                if (floatingLabelTransform != null)
                {
                    float labelScale = 1.0f + pulse * 0.15f;
                    floatingLabelTransform.localScale = Vector3.one * (0.8f * labelScale);
                }
            }
            else
            {
                if (highlightLight != null && highlightLight.enabled)
                {
                    highlightLight.enabled = false;
                }

                if (bodyMatInstance != null)
                {
                    bodyMatInstance.SetColor("_EmissionColor", Color.black);
                    bodyMatInstance.DisableKeyword("_EMISSION");
                }

                if (floatingLabelTransform != null)
                {
                    floatingLabelTransform.localScale = Vector3.one * 0.8f;
                }
            }
        }

        public void SetPlayerNearby(bool nearby)
        {
            isPlayerNearby = nearby;
        }

        public void SetHighlight(bool highlight)
        {
            isHighlighted = highlight;
        }

        public bool TryDeposit(TrashItem item)
        {
            if (item == null) return false;

            bool isCorrect = (item.Category == acceptedCategory);

            if (isCorrect)
            {
                if (correctEffect != null) correctEffect.Play();
            }
            else
            {
                if (incorrectEffect != null) incorrectEffect.Play();
            }

            OnTrashDeposited?.Invoke(isCorrect, item);
            item.OnDeposited();
            return isCorrect;
        }
    }
}
