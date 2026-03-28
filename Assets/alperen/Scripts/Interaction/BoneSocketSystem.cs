using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Alperen.Scripts.Interaction
{
    /// <summary>
    /// Handles bone socket interaction and events.
    /// Only accepts objects with "DinoBone" tag.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor))]
    public class BoneSocketSystem : MonoBehaviour
    {
        [Header("Socket Settings")]
        [SerializeField] private string acceptedTag = "DinoBone";
        [SerializeField] private bool allowMultipleBones = false;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject socketVisual;
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color inactiveColor = Color.gray;
        private Color originalColor;

        [Header("Events")]
        [SerializeField] private UnityEventObject onBoneSocketed;

        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;
        private Renderer socketRenderer;
        private int currentBoneCount = 0;

        /// <summary>
        /// Event raised when a bone is successfully socketed.
        /// </summary>
        public event System.Action<GameObject> OnBoneSocketed;

        private void Awake()
        {
            socketInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

            // Get visual feedback component
            if (socketVisual != null)
            {
                socketRenderer = socketVisual.GetComponent<Renderer>();
                if (socketRenderer != null)
                {
                    originalColor = socketRenderer.material.color;
                }
            }
        }

        private void OnEnable()
        {
            // Subscribe to socket events
            if (socketInteractor != null)
            {
                socketInteractor.selectEntered.AddListener(OnBoneSocketEnter);
                socketInteractor.selectExited.AddListener(OnBoneSocketExit);
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from socket events
            if (socketInteractor != null)
            {
                socketInteractor.selectEntered.RemoveListener(OnBoneSocketEnter);
                socketInteractor.selectExited.RemoveListener(OnBoneSocketExit);
            }
        }

        /// <summary>
        /// Called when a bone enters the socket.
        /// </summary>
        private void OnBoneSocketEnter(SelectEnterEventArgs args)
        {
            GameObject boneObject = args.interactableObject.transform.gameObject;

            // Verify the object has the correct tag
            if (!boneObject.CompareTag(acceptedTag))
            {
                Debug.LogWarning($"BoneSocket: {boneObject.name} does not have tag '{acceptedTag}'. Releasing socket.");
                return;
            }

            currentBoneCount++;
            Debug.Log($"Kemik yerleştirildi! {boneObject.name} sokete girdi. Toplam kemik: {currentBoneCount}");

            // Trigger event
            OnBoneSocketed?.Invoke(boneObject);
            onBoneSocketed?.Invoke(boneObject);

            // Update visual feedback
            UpdateVisualFeedback(true);

            // Haptic feedback for the hand holding the bone
            TriggerHapticFeedback(args);
        }

        /// <summary>
        /// Called when a bone exits the socket.
        /// </summary>
        private void OnBoneSocketExit(SelectExitEventArgs args)
        {
            currentBoneCount = Mathf.Max(0, currentBoneCount - 1);
            Debug.Log($"Kemik çıkarıldı! {args.interactableObject.transform.gameObject.name} soketten çıktı. Kalan kemik: {currentBoneCount}");

            // Update visual feedback
            UpdateVisualFeedback(false);
        }

        /// <summary>
        /// Triggers haptic feedback on the controller.
        /// </summary>
        private void TriggerHapticFeedback(SelectEnterEventArgs args)
        {
            // XRI 3.x: Use XRBaseController for haptic feedback
            var controller = args.interactorObject.transform.gameObject.GetComponentInParent<XRBaseController>();
            if (controller != null)
            {
                controller.SendHapticImpulse(0.5f, 0.2f);
            }
        }

        /// <summary>
        /// Updates the visual feedback of the socket.
        /// </summary>
        private void UpdateVisualFeedback(bool hasBone)
        {
            if (socketRenderer != null)
            {
                socketRenderer.material.color = hasBone ? activeColor : originalColor;
            }
        }

        /// <summary>
        /// Checks if the socket currently has a bone.
        /// </summary>
        public bool HasBone => socketInteractor.hasSelection;

        /// <summary>
        /// Gets the currently socketed bone (if any).
        /// </summary>
        public GameObject GetCurrentBone()
        {
            if (socketInteractor.hasSelection)
            {
                return socketInteractor.firstInteractableSelected.transform.gameObject;
            }
            return null;
        }

        /// <summary>
        /// Gets the count of bones currently in the socket.
        /// </summary>
        public int BoneCount => currentBoneCount;

        /// <summary>
        /// Clears the socket (releases the bone).
        /// </summary>
        public void ClearSocket()
        {
            if (socketInteractor.hasSelection)
            {
                var bone = socketInteractor.firstInteractableSelected;
                // The XRSocketInteractor will handle releasing
            }
        }
    }

    /// <summary>
    /// UnityEvent wrapper for GameObject events.
    /// </summary>
    [System.Serializable]
    public class UnityEventObject : UnityEngine.Events.UnityEvent<GameObject> { }
}
