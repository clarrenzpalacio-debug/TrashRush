using UnityEngine;
using TrashRush.Core;

namespace TrashRush.Trash
{
    public class TrashItem : MonoBehaviour
    {
        [Header("Trash Configuration")]
        [SerializeField] private TrashCategory category;
        [SerializeField] private string itemName = "Trash Item";
        [SerializeField] private int pointValue = 10;

        [Header("State")]
        private bool isCollected = false;
        private Collider itemCollider;
        private Rigidbody itemRigidbody;
        private Renderer itemRenderer;

        public TrashCategory Category => category;
        public string ItemName => itemName;
        public int PointValue => pointValue;
        public bool IsCollected => isCollected;

        private void Awake()
        {
            itemCollider = GetComponent<Collider>();
            itemRigidbody = GetComponent<Rigidbody>();
            itemRenderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            // Subtle idle rotate while uncollected on the ground to aid player discoverability
            if (!isCollected)
            {
                transform.Rotate(Vector3.up, 30f * Time.deltaTime, Space.World);
            }
        }

        public void SetData(TrashCategory cat, string name)
        {
            category = cat;
            itemName = name;
        }

        public void OnPickedUp(Transform holdPoint)
        {
            isCollected = true;

            // Disable physics interaction while carried
            if (itemCollider != null) itemCollider.enabled = false;
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = true;
                itemRigidbody.useGravity = false;
            }

            // Ensure mesh remains fully active and visible
            if (itemRenderer != null) itemRenderer.enabled = true;
            gameObject.SetActive(true);

            // Parent to player's carry point (held visible in front/hands)
            transform.SetParent(holdPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * 1.15f; // slightly prominent for clear visibility
        }

        public void OnDropped(Vector3 worldPos)
        {
            isCollected = false;
            transform.SetParent(null);
            transform.position = worldPos;
            transform.localScale = Vector3.one;

            if (itemCollider != null) itemCollider.enabled = true;
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = true;
            }
            if (itemRenderer != null) itemRenderer.enabled = true;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Called when the item is stored in the trash bag inventory.
        /// Hides the item and disables physics, but keeps it alive so we
        /// retain its identity (category, name, points) until deposited.
        /// </summary>
        public void OnStoredInBag()
        {
            isCollected = true;

            // Disable physics
            if (itemCollider != null) itemCollider.enabled = false;
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = true;
                itemRigidbody.useGravity = false;
            }

            // Un-parent from anything and hide
            transform.SetParent(null);
            gameObject.SetActive(false);
        }

        public void OnDeposited()
        {
            Destroy(gameObject);
        }
    }
}
