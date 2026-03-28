using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

namespace Alperen.Scripts.Interaction
{
    /// <summary>
    /// Handles digging mechanics for the shovel tool.
    /// Detects collision with soil layer and spawns particle effects.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class DiggingSystem : MonoBehaviour
    {
        [Header("Digging Settings")]
        [SerializeField] private LayerMask soilLayer = 1 << 6; // Layer 6 = Soil by default
        [SerializeField] private float minDigVelocity = 0.5f; // Minimum velocity to trigger dig
        [SerializeField] private float digCooldown = 0.2f; // Cooldown between dig triggers

        [Header("Particle Effects")]
        [SerializeField] private GameObject soilParticlePrefab;
        [SerializeField] private int particleCount = 10;
        [SerializeField] private float particleSpread = 0.3f;
        [SerializeField] private float particleLifetime = 1.5f;

        [Header("Audio")]
        [SerializeField] private AudioClip digSound;

        private Collider shovelCollider;
        private Rigidbody shovelRigidbody;
        private AudioSource audioSource;
        private float lastDigTime;
        private Vector3 lastPosition;

        private void Awake()
        {
            shovelCollider = GetComponent<Collider>();
            shovelRigidbody = GetComponent<Rigidbody>();
            lastPosition = transform.position;

            // Setup audio source if needed
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void Update()
        {
            // Update position for velocity calculation
            lastPosition = transform.position;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Check if collision is with soil layer
            if (((1 << collision.gameObject.layer) & soilLayer) != 0)
            {
                // Check if moving fast enough
                Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
                if (velocity.magnitude >= minDigVelocity)
                {
                    // Check cooldown
                    if (Time.time >= lastDigTime + digCooldown)
                    {
                        PerformDig(collision.contacts[0].point, collision.contacts[0].normal, collision.gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Performs the digging action at the specified point.
        /// </summary>
        /// <param name="contactPoint">The point where the shovel hit the soil.</param>
        /// <param name="contactNormal">The normal of the surface hit.</param>
        /// <param name="soilObject">The soil object that was hit.</param>
        private void PerformDig(Vector3 contactPoint, Vector3 contactNormal, GameObject soilObject)
        {
            lastDigTime = Time.time;

            // Log the dig action
            Debug.Log($"Kazı yapıldı - Pozisyon: {contactPoint} - Hedef: {soilObject.name}");

            // Apply hit to the soil cube
            SoilCube soilCube = soilObject.GetComponent<SoilCube>();
            if (soilCube != null)
            {
                soilCube.TakeHit();
            }

            // Spawn particle effects
            SpawnSoilParticles(contactPoint, contactNormal);

            // Play dig sound
            if (digSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(digSound);
            }

            // Trigger dig event (can be connected to other systems)
            OnDig?.Invoke(contactPoint);
        }

        /// <summary>
        /// Spawns soil particles at the dig location.
        /// </summary>
        private void SpawnSoilParticles(Vector3 position, Vector3 normal)
        {
            // If no prefab is assigned, we can create a simple effect programmatically
            if (soilParticlePrefab == null)
            {
                CreateSimpleParticleEffect(position, normal);
                return;
            }

            // Spawn the particle prefab
            Instantiate(soilParticlePrefab, position, Quaternion.LookRotation(normal));
        }

        /// <summary>
        /// Creates a simple particle effect without a prefab.
        /// </summary>
        private void CreateSimpleParticleEffect(Vector3 position, Vector3 normal)
        {
            // Create temporary particles
            for (int i = 0; i < particleCount; i++)
            {
                GameObject particle = new GameObject("SoilParticle");
                particle.transform.position = position;

                // Random direction spread
                Vector3 randomDir = normal + new Vector3(
                    Random.Range(-particleSpread, particleSpread),
                    Random.Range(-particleSpread, particleSpread),
                    Random.Range(-particleSpread, particleSpread)
                );

                Rigidbody particleRb = particle.AddComponent<Rigidbody>();
                particleRb.mass = 0.1f;
                particleRb.drag = 1f;
                particleRb.angularDrag = 5f;

                // Give initial velocity
                float force = Random.Range(1f, 3f);
                particleRb.AddForce(randomDir.normalized * force, ForceMode.Impulse);

                // Add simple renderer
                MeshRenderer renderer = particle.AddComponent<MeshRenderer>();
                MeshFilter filter = particle.AddComponent<MeshFilter>();
                filter.mesh = CreateCubeMesh();
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.color = new Color(0.4f, 0.3f, 0.2f, 1f); // Brown soil color

                // Scale particle
                particle.transform.localScale = Vector3.one * Random.Range(0.05f, 0.15f);

                // Destroy after lifetime
                Destroy(particle, particleLifetime);
            }
        }

        /// <summary>
        /// Creates a simple cube mesh for particles.
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
        /// Event raised when a dig action is performed.
        /// </summary>
        public event System.Action<Vector3> OnDig;

        /// <summary>
        /// Checks if digging is currently available (not on cooldown).
        /// </summary>
        public bool CanDig => Time.time >= lastDigTime + digCooldown;
    }
}
