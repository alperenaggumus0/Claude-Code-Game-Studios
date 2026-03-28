using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Alperen.Scripts.Interaction
{
    /// <summary>
    /// Controls flashlight on/off functionality via XR Activate events.
    /// Toggles the spotlight when the trigger button is pressed.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class FlashlightController : MonoBehaviour
    {
        [Header("Flashlight Settings")]
        [SerializeField] private Light spotlight;
        [SerializeField] private GameObject flashlightGlow;
        [SerializeField] private float flickerChance = 0f;
        [SerializeField] private float batteryDrainRate = 0f;

        [Header("Audio")]
        [SerializeField] private AudioClip toggleOnSound;
        [SerializeField] private AudioClip toggleOffSound;
        [SerializeField] private AudioClip flickerSound;

        [Header("Visual Effects")]
        [SerializeField] private Color onColor = Color.white;
        [SerializeField] private Color offColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        [Header("Events")]
        [SerializeField] private UnityEngine.Events.UnityEvent onFlashlightOn;
        [SerializeField] private UnityEngine.Events.UnityEvent onFlashlightOff;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
        private Renderer flashlightRenderer;
        private AudioSource audioSource;
        private bool isOn = false;
        private float batteryLevel = 1f;
        private float originalIntensity;
        private Color originalEmissionColor;

        /// <summary>
        /// Event raised when the flashlight is turned on.
        /// </summary>
        public event System.Action OnFlashlightOn;

        /// <summary>
        /// Event raised when the flashlight is turned off.
        /// </summary>
        public event System.Action OnFlashlightOff;

        private void Awake()
        {
            grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            audioSource = GetComponent<AudioSource>();

            // Find the spotlight if not assigned
            if (spotlight == null)
            {
                spotlight = GetComponentInChildren<Light>();
                if (spotlight == null)
                {
                    Debug.LogWarning("FlashlightController: No spotlight found! Please assign one.");
                }
            }

            // Get renderer for visual feedback
            flashlightRenderer = GetComponentInChildren<Renderer>();
            if (flashlightRenderer != null)
            {
                originalEmissionColor = flashlightRenderer.material.GetColor("_EmissionColor");
            }

            // Store original intensity
            if (spotlight != null)
            {
                originalIntensity = spotlight.intensity;
            }

            // Setup audio source if needed
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f; // Full 3D sound
            }
        }

        private void OnEnable()
        {
            // Subscribe to XR events
            grabInteractable.activated.AddListener(ToggleFlashlight);
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }

        private void OnDisable()
        {
            // Unsubscribe from XR events
            grabInteractable.activated.RemoveListener(ToggleFlashlight);
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }

        private void Start()
        {
            // Start with flashlight off
            SetFlashlightState(false);
        }

        /// <summary>
        /// Called when the flashlight is grabbed.
        /// </summary>
        private void OnGrabbed(SelectEnterEventArgs args)
        {
            // Optional: Auto-turn on when grabbed
            // ToggleFlashlight(new ActivateEventArgs());
        }

        /// <summary>
        /// Called when the flashlight is released.
        /// </summary>
        private void OnReleased(SelectExitEventArgs args)
        {
            // Optional: Auto-turn off when released
            // SetFlashlightState(false);
        }

        /// <summary>
        /// Toggles the flashlight on/off when Activate event is triggered.
        /// </summary>
        private void ToggleFlashlight(ActivateEventArgs args)
        {
            SetFlashlightState(!isOn);
        }

        /// <summary>
        /// Sets the flashlight state (on/off).
        /// </summary>
        /// <param name="turnOn">True to turn on, false to turn off.</param>
        public void SetFlashlightState(bool turnOn)
        {
            isOn = turnOn;

            // Toggle spotlight
            if (spotlight != null)
            {
                spotlight.enabled = isOn;

                // Handle battery drain
                if (isOn && batteryDrainRate > 0)
                {
                    spotlight.intensity = originalIntensity * batteryLevel;
                }
            }

            // Toggle glow effect
            if (flashlightGlow != null)
            {
                flashlightGlow.SetActive(isOn);
            }

            // Update visual feedback
            UpdateVisualFeedback();

            // Play sound
            PlayToggleSound();

            // Trigger haptic feedback
            TriggerHapticFeedback();

            // Invoke events
            if (isOn)
            {
                onFlashlightOn?.Invoke();
                OnFlashlightOn?.Invoke();
                Debug.Log("Flashlight: Açıldı!");
            }
            else
            {
                onFlashlightOff?.Invoke();
                OnFlashlightOff?.Invoke();
                Debug.Log("Flashlight: Kapatıldı!");
            }
        }

        /// <summary>
        /// Updates visual feedback on the flashlight model.
        /// </summary>
        private void UpdateVisualFeedback()
        {
            if (flashlightRenderer != null)
            {
                // Change emission color to indicate state
                Color targetColor = isOn ? onColor : offColor;
                if (flashlightRenderer.material.HasProperty("_EmissionColor"))
                {
                    flashlightRenderer.material.SetColor("_EmissionColor", targetColor);
                }
                if (flashlightRenderer.material.HasProperty("_EmissionIntensity"))
                {
                    flashlightRenderer.material.SetFloat("_EmissionIntensity", isOn ? 1f : 0.2f);
                }
            }
        }

        /// <summary>
        /// Plays the toggle sound.
        /// </summary>
        private void PlayToggleSound()
        {
            if (audioSource != null)
            {
                if (isOn && toggleOnSound != null)
                {
                    audioSource.PlayOneShot(toggleOnSound);
                }
                else if (!isOn && toggleOffSound != null)
                {
                    audioSource.PlayOneShot(toggleOffSound);
                }
            }
        }

        /// <summary>
        /// Triggers haptic feedback on the controller.
        /// </summary>
        private void TriggerHapticFeedback()
        {
            // XRI 3.x: Use XRBaseController for haptic feedback
            if (grabInteractable.firstInteractorSelecting != null)
            {
                var controller = grabInteractable.firstInteractorSelecting.transform.gameObject
                    .GetComponentInParent<XRBaseController>();

                if (controller != null)
                {
                    controller.SendHapticImpulse(isOn ? 0.8f : 0.3f, 0.15f);
                }
            }
        }

        /// <summary>
        /// Updates battery level (for battery mechanic).
        /// </summary>
        private void Update()
        {
            if (isOn && batteryDrainRate > 0)
            {
                batteryLevel -= batteryDrainRate * Time.deltaTime;
                batteryLevel = Mathf.Clamp01(batteryLevel);

                if (spotlight != null)
                {
                    spotlight.intensity = originalIntensity * batteryLevel;
                }

                // Flicker effect when battery is low
                if (batteryLevel < 0.3f && Random.value < flickerChance)
                {
                    Flicker();
                }

                // Turn off when battery is empty
                if (batteryLevel <= 0f)
                {
                    SetFlashlightState(false);
                }
            }
        }

        /// <summary>
        /// Creates a flicker effect on the flashlight.
        /// </summary>
        private void Flicker()
        {
            if (spotlight != null)
            {
                spotlight.enabled = false;
                Invoke(nameof(ResetFlicker), Random.Range(0.05f, 0.15f));
            }

            if (flickerSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(flickerSound, 0.5f);
            }
        }

        private void ResetFlicker()
        {
            if (spotlight != null && isOn)
            {
                spotlight.enabled = true;
            }
        }

        /// <summary>
        /// Gets the current flashlight state.
        /// </summary>
        public bool IsOn => isOn;

        /// <summary>
        /// Gets the current battery level (0-1).
        /// </summary>
        public float BatteryLevel => batteryLevel;

        /// <summary>
        /// Sets the battery level.
        /// </summary>
        public void SetBatteryLevel(float level)
        {
            batteryLevel = Mathf.Clamp01(level);
        }

        /// <summary>
        /// Replaces the battery (resets to full).
        /// </summary>
        public void ReplaceBattery()
        {
            batteryLevel = 1f;
            Debug.Log("Flashlight: Pil değiştirildi!");
        }
    }
}
