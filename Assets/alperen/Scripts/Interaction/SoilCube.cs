using UnityEngine;

namespace Alperen.Scripts.Interaction
{
    /// <summary>
    /// Represents a single soil cube that can be dug by the shovel.
    /// Tracks hits, reduces scale, and deactivates itself when destroyed.
    /// </summary>
    public class SoilCube : MonoBehaviour
    {
        [Header("Soil Settings")]
        [SerializeField] private int requiredHits = 3; // Hits needed to destroy this cube
        [SerializeField] private bool startActive = true; // Should this cube start active?

        private int currentHits;
        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
            currentHits = 0;

            // Start inactive if configured
            if (!startActive)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Called when the shovel hits this soil cube.
        /// Reduces scale and checks if the cube should be destroyed.
        /// </summary>
        public void TakeHit()
        {
            if (currentHits >= requiredHits) return;

            currentHits++;

            // Calculate new scale based on progress
            float progress = (float)currentHits / requiredHits;
            Vector3 newScale = originalScale * (1f - progress);
            transform.localScale = newScale;

            Debug.Log($"{gameObject.name} vuruş: {currentHits}/{requiredHits} - Scale: {newScale.x:F2}");

            // Check if destroyed
            if (currentHits >= requiredHits)
            {
                gameObject.SetActive(false);
                Debug.Log($"{gameObject.name} yok edildi!");
            }
        }

        /// <summary>
        /// Resets the soil cube to its original state.
        /// </summary>
        public void ResetCube()
        {
            currentHits = 0;
            transform.localScale = originalScale;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Gets the current progress (0.0 to 1.0).
        /// </summary>
        public float Progress => (float)currentHits / requiredHits;

        /// <summary>
        /// Gets whether this cube is completely destroyed.
        /// </summary>
        public bool IsDestroyed => currentHits >= requiredHits;
    }
}
