using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Alperen.Scripts.UI;

namespace Alperen.Scripts.Interaction
{
    /// <summary>
    /// General feedback manager for all VR interactions.
    /// Handles haptic feedback, audio feedback, and particle effects.
    /// </summary>
    public class FeedbackManager : MonoBehaviour
    {
        [Header("Singleton")]
        [SerializeField] private bool dontDestroyOnLoad = false;

        [Header("Haptic Settings")]
        [SerializeField] private float digHapticIntensity = 0.3f;
        [SerializeField] private float digHapticDuration = 0.1f;
        [SerializeField] private float successHapticIntensity = 0.8f;
        [SerializeField] private float successHapticDuration = 0.3f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip digSound;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private AudioClip dropSound;
        [SerializeField] private float volume = 1f;

        [Header("Particle Effects")]
        [SerializeField] private GameObject soilParticlePrefab;
        [SerializeField] private int soilParticleCount = 15;
        [SerializeField] private float particleLifetime = 2f;
        [SerializeField] private float particleSpread = 0.2f;

        [Header("Bone Socket Connection")]
        [SerializeField] private BoneSocketSystem boneSocketSystem;

        [Header("Audio Source Pool")]
        [SerializeField] private int audioSourcePoolSize = 5;

        private AudioSource[] audioSourcePool;
        private int currentAudioSourceIndex = 0;
        private XRController activeController;
        private float lastDigTime = 0f;
        private float digCooldown = 0.15f;

        // Singleton instance
        private static FeedbackManager instance;
        public static FeedbackManager Instance => instance;

        /// <summary>
        /// Event raised when dig feedback is triggered.
        /// </summary>
        public event System.Action OnDigFeedback;

        /// <summary>
        /// Event raised when success feedback is triggered.
        /// </summary>
        public event System.Action OnSuccessFeedback;

        private void Awake()
        {
            // Singleton setup
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            // Initialize audio source pool
            InitializeAudioSourcePool();

            // Find BoneSocketSystem if not assigned
            if (boneSocketSystem == null)
            {
                boneSocketSystem = FindObjectOfType<BoneSocketSystem>();
                if (boneSocketSystem != null)
                {
                    Debug.Log($"FeedbackManager: BoneSocketSystem bulundu: {boneSocketSystem.name}");
                }
            }
        }

        private void Start()
        {
            // Subscribe to bone socket events
            if (boneSocketSystem != null)
            {
                boneSocketSystem.OnBoneSocketed += OnBoneSocketed;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (boneSocketSystem != null)
            {
                boneSocketSystem.OnBoneSocketed -= OnBoneSocketed;
            }
        }

        private void Update()
        {
            // Find active XR controller for haptics
            if (activeController == null)
            {
                FindActiveController();
            }
        }

        /// <summary>
        /// Initializes the audio source pool for efficient audio playback.
        /// </summary>
        private void InitializeAudioSourcePool()
        {
            audioSourcePool = new AudioSource[audioSourcePoolSize];
            GameObject poolContainer = new GameObject("AudioSourcePool");

            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                GameObject audioObj = new GameObject($"AudioSource_{i}");
                audioObj.transform.parent = poolContainer.transform;

                AudioSource source = audioObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 1f; // Full 3D sound
                source.maxDistance = 20f;
                source.rolloffMode = AudioRolloffMode.Linear;

                audioSourcePool[i] = source;
            }
        }

        /// <summary>
        /// Finds the active XR controller for haptic feedback.
        /// </summary>
        private void FindActiveController()
        {
            var controllers = FindObjectsOfType<XRController>();
            if (controllers != null && controllers.Length > 0)
            {
                activeController = controllers[0]; // Use first controller
            }
        }

        /// <summary>
        /// Plays a sound from the audio pool.
        /// </summary>
        private void PlaySound(AudioClip clip, Vector3 position)
        {
            if (clip == null || audioSourcePool == null) return;

            // Check cooldown for dig sounds
            if (clip == digSound && Time.time < lastDigTime + digCooldown)
            {
                return;
            }

            AudioSource source = audioSourcePool[currentAudioSourceIndex];
            source.transform.position = position;
            source.PlayOneShot(clip, volume);

            currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSourcePoolSize;

            if (clip == digSound)
            {
                lastDigTime = Time.time;
            }
        }

        /// <summary>
        /// Triggers haptic feedback on the active controller.
        /// </summary>
        private void TriggerHaptic(float intensity, float duration)
        {
            if (activeController != null)
            {
                activeController.SendHapticImpulse(intensity, duration);
            }
        }

        /// <summary>
        /// Called when a bone is socketed.
        /// </summary>
        private void OnBoneSocketed(GameObject bone)
        {
            // Trigger success feedback
            TriggerSuccessFeedback();
        }

        /// <summary>
        /// Triggers dig feedback at the specified position.
        /// </summary>
        /// <param name="position">The position where the dig occurred.</param>
        public void TriggerDigFeedback(Vector3 position)
        {
            // Haptic feedback
            TriggerHaptic(digHapticIntensity, digHapticDuration);

            // Audio feedback
            PlaySound(digSound, position);

            // Particle effects
            SpawnSoilParticles(position);

            // Trigger event
            OnDigFeedback?.Invoke();

            Debug.Log($"FeedbackManager: Kazı feedback tetiklendi - Pozisyon: {position}");
        }

        /// <summary>
        /// Triggers success feedback.
        /// </summary>
        public void TriggerSuccessFeedback()
        {
            // Strong haptic feedback (double pulse)
            TriggerHaptic(successHapticIntensity, successHapticDuration);
            Invoke(nameof(TriggerSuccessHapticPulse), 0.15f);

            // Audio feedback
            PlaySound(successSound, Vector3.zero);

            // Trigger event
            OnSuccessFeedback?.Invoke();

            Debug.Log("FeedbackManager: Başarı feedback tetiklendi!");
        }

        /// <summary>
        /// Triggers a secondary haptic pulse for success feedback.
        /// </summary>
        private void TriggerSuccessHapticPulse()
        {
            TriggerHaptic(successHapticIntensity * 0.6f, successHapticDuration * 0.6f);
        }

        /// <summary>
        /// Triggers pickup feedback.
        /// </summary>
        public void TriggerPickupFeedback()
        {
            // Light haptic feedback
            TriggerHaptic(0.2f, 0.1f);

            // Audio feedback
            PlaySound(pickupSound, Vector3.zero);

            Debug.Log("FeedbackManager: Pickup feedback tetiklendi");
        }

        /// <summary>
        /// Triggers drop feedback.
        /// </summary>
        public void TriggerDropFeedback()
        {
            // Light haptic feedback
            TriggerHaptic(0.15f, 0.1f);

            // Audio feedback
            PlaySound(dropSound, Vector3.zero);

            Debug.Log("FeedbackManager: Drop feedback tetiklendi");
        }

        /// <summary>
        /// Spawns soil particles at the specified position.
        /// </summary>
        private void SpawnSoilParticles(Vector3 position)
        {
            if (soilParticlePrefab != null)
            {
                Instantiate(soilParticlePrefab, position, Quaternion.identity);
                return;
            }

            // Create simple particles programmatically
            CreateSimpleParticles(position);
        }

        /// <summary>
        /// Creates simple soil particles programmatically.
        /// </summary>
        private void CreateSimpleParticles(Vector3 position)
        {
            for (int i = 0; i < soilParticleCount; i++)
            {
                GameObject particle = new GameObject("SoilParticle");
                particle.transform.position = position;

                // Random direction
                Vector3 randomDir = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.2f, 1f), // Prefer upward direction
                    Random.Range(-1f, 1f)
                ).normalized;

                Rigidbody rb = particle.AddComponent<Rigidbody>();
                rb.mass = 0.05f;
                rb.drag = 1.5f;
                rb.angularDrag = 5f;

                // Initial velocity
                float force = Random.Range(1.5f, 3f);
                rb.AddForce(randomDir * force, ForceMode.Impulse);

                // Add rotation
                rb.AddTorque(new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ), ForceMode.Impulse);

                // Visual component
                MeshRenderer renderer = particle.AddComponent<MeshRenderer>();
                MeshFilter filter = particle.AddComponent<MeshFilter>();
                filter.mesh = CreateCubeMesh();

                // Brown soil color with variation
                Color soilColor = new Color(
                    0.4f + Random.Range(-0.1f, 0.1f),
                    0.3f + Random.Range(-0.1f, 0.1f),
                    0.2f + Random.Range(-0.1f, 0.1f),
                    1f
                );

                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = soilColor;
                renderer.material.SetFloat("_Metallic", 0f);
                renderer.material.SetFloat("_Smoothness", 0.1f);

                // Random scale
                float scale = Random.Range(0.05f, 0.15f);
                particle.transform.localScale = Vector3.one * scale;

                // Destroy after lifetime
                Destroy(particle, particleLifetime);
            }
        }

        /// <summary>
        /// Creates a simple cube mesh.
        /// </summary>
        private Mesh CreateCubeMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f)
            };
            mesh.triangles = new int[]
            {
                0, 2, 1, 0, 3, 2,
                1, 2, 6, 1, 6, 5,
                5, 6, 7, 5, 7, 4,
                4, 7, 3, 4, 3, 0,
                3, 7, 6, 3, 6, 2,
                4, 0, 1, 4, 1, 5
            };
            mesh.RecalculateNormals();
            return mesh;
        }

        /// <summary>
        /// Sets the active controller for haptic feedback.
        /// </summary>
        public void SetActiveController(XRController controller)
        {
            activeController = controller;
        }

        /// <summary>
        /// Gets the current active controller.
        /// </summary>
        public XRController GetActiveController() => activeController;

        /// <summary>
        /// Sets the dig haptic intensity.
        /// </summary>
        public void SetDigHapticIntensity(float intensity)
        {
            digHapticIntensity = Mathf.Clamp01(intensity);
        }

        /// <summary>
        /// Sets the success haptic intensity.
        /// </summary>
        public void SetSuccessHapticIntensity(float intensity)
        {
            successHapticIntensity = Mathf.Clamp01(intensity);
        }

        /// <summary>
        /// Sets the global audio volume.
        /// </summary>
        public void SetVolume(float vol)
        {
            volume = Mathf.Clamp01(vol);
        }

        /// <summary>
        /// Gets the current audio volume.
        /// </summary>
        public float Volume => volume;
    }
}
