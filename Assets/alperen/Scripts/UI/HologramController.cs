using UnityEngine;

namespace Alperen.Scripts.UI
{
    /// <summary>
    /// Controls the hologram effect for the dinosaur model.
    /// Handles rotation, scaling, and visual hologram effects.
    /// </summary>
    public class HologramController : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private bool autoRotate = true;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private bool randomizeRotation = false;

        [Header("Hologram Effect")]
        [SerializeField] private bool useHologramEffect = true;
        [SerializeField] private float hologramFlickerSpeed = 0.1f;
        [SerializeField] private float hologramFlickerIntensity = 0.1f;
        [SerializeField] private Color hologramColor = new Color(0.5f, 0.8f, 1f, 0.6f);

        [Header("Scale Settings")]
        [SerializeField] private bool pulseScale = false;
        [SerializeField] private float pulseSpeed = 1f;
        [SerializeField] private float pulseMinScale = 0.95f;
        [SerializeField] private float pulseMaxScale = 1.05f;

        [Header("Particle Effect")]
        [SerializeField] private bool useParticles = true;
        [SerializeField] private GameObject hologramParticles;
        [SerializeField] private int particleCount = 20;
        [SerializeField] private float particleLifetime = 2f;

        [Header("Model Display")]
        [SerializeField] private Transform modelTransform;
        [SerializeField] private bool showWireframe = false;

        private MeshRenderer[] modelRenderers;
        private Material[] originalMaterials;
        private Material hologramMaterial;
        private float currentRotationAngle;
        private float pulseTime;
        private float flickerTime;

        private void Awake()
        {
            // Get model renderers
            if (modelTransform != null)
            {
                modelRenderers = modelTransform.GetComponentsInChildren<MeshRenderer>();
            }
            else
            {
                // Use own transform if not assigned
                modelRenderers = GetComponentsInChildren<MeshRenderer>();
            }

            // Store original materials
            if (modelRenderers != null && modelRenderers.Length > 0)
            {
                originalMaterials = new Material[modelRenderers.Length];
                for (int i = 0; i < modelRenderers.Length; i++)
                {
                    if (modelRenderers[i] != null && modelRenderers[i].materials != null && modelRenderers[i].materials.Length > 0)
                    {
                        originalMaterials[i] = modelRenderers[i].materials[0];
                    }
                }
            }

            // Create hologram material
            CreateHologramMaterial();
        }

        private void Start()
        {
            // Create particles if needed
            if (useParticles && hologramParticles == null)
            {
                CreateHologramParticles();
            }

            // Apply hologram material
            if (useHologramEffect)
            {
                ApplyHologramMaterial();
            }
        }

        private void Update()
        {
            // Auto rotate
            if (autoRotate)
            {
                UpdateRotation();
            }

            // Pulse scale
            if (pulseScale)
            {
                UpdatePulse();
            }

            // Hologram flicker
            if (useHologramEffect && hologramFlickerIntensity > 0)
            {
                UpdateFlicker();
            }
        }

        /// <summary>
        /// Updates the hologram rotation.
        /// </summary>
        private void UpdateRotation()
        {
            Vector3 axis = rotationAxis;
            if (randomizeRotation)
            {
                axis = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0.5f, 1f),
                    Random.Range(-0.5f, 0.5f)
                ).normalized;
            }

            currentRotationAngle += rotationSpeed * Time.deltaTime;
            if (modelTransform != null)
            {
                modelTransform.localRotation = Quaternion.AngleAxis(currentRotationAngle, axis);
            }
            else
            {
                transform.localRotation = Quaternion.AngleAxis(currentRotationAngle, axis);
            }
        }

        /// <summary>
        /// Updates the pulse scale effect.
        /// </summary>
        private void UpdatePulse()
        {
            pulseTime += Time.deltaTime * pulseSpeed;
            float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale, (Mathf.Sin(pulseTime) + 1f) * 0.5f);

            if (modelTransform != null)
            {
                Vector3 baseScale = modelTransform.localScale;
                modelTransform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        /// <summary>
        /// Updates the hologram flicker effect.
        /// </summary>
        private void UpdateFlicker()
        {
            flickerTime += Time.deltaTime * hologramFlickerSpeed;

            if (hologramMaterial != null)
            {
                float flicker = 1f + (Random.Range(-1f, 1f) * hologramFlickerIntensity);
                Color currentColor = hologramMaterial.GetColor("_EmissionColor");
                currentColor.a = Mathf.Clamp01(currentColor.a * flicker);
                hologramMaterial.SetColor("_EmissionColor", currentColor);
            }
        }

        /// <summary>
        /// Creates the hologram material.
        /// </summary>
        private void CreateHologramMaterial()
        {
            if (originalMaterials != null && originalMaterials.Length > 0 && originalMaterials[0] != null)
            {
                hologramMaterial = new Material(originalMaterials[0]);
            }
            else
            {
                // Create default material
                hologramMaterial = new Material(Shader.Find("Standard"));
            }

            // Set hologram properties
            hologramMaterial.SetFloat("_Metallic", 0f);
            hologramMaterial.SetFloat("_Smoothness", 0.8f);
            hologramMaterial.SetFloat("_Glossiness", 0.8f);

            // Set emission for glow effect
            hologramMaterial.SetColor("_EmissionColor", hologramColor);

            // Enable transparency
            hologramMaterial.SetFloat("_Mode", 2); // Transparent mode
            hologramMaterial.SetFloat("_SrcBlend", 5); // SrcAlpha
            hologramMaterial.SetFloat("_DstBlend", 10); // OneMinusSrcAlpha
            hologramMaterial.SetFloat("_ZWrite", 0); // Off
            hologramMaterial.DisableKeyword("_ALPHATEST_ON");
            hologramMaterial.EnableKeyword("_ALPHABLEND_ON");
            hologramMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            hologramMaterial.renderQueue = 3000;
        }

        /// <summary>
        /// Applies the hologram material to all renderers.
        /// </summary>
        public void ApplyHologramMaterial()
        {
            if (hologramMaterial == null || modelRenderers == null) return;

            foreach (var renderer in modelRenderers)
            {
                if (renderer != null && renderer.materials != null)
                {
                    for (int i = 0; i < renderer.materials.Length; i++)
                    {
                        renderer.materials[i] = hologramMaterial;
                    }
                }
            }
        }

        /// <summary>
        /// Restores the original materials.
        /// </summary>
        public void RestoreOriginalMaterials()
        {
            if (originalMaterials == null || modelRenderers == null) return;

            for (int i = 0; i < Mathf.Min(modelRenderers.Length, originalMaterials.Length); i++)
            {
                if (modelRenderers[i] != null && originalMaterials[i] != null)
                {
                    modelRenderers[i].material = originalMaterials[i];
                }
            }
        }

        /// <summary>
        /// Creates hologram particle effect.
        /// </summary>
        private void CreateHologramParticles()
        {
            GameObject particleContainer = new GameObject("HologramParticles");
            particleContainer.transform.parent = transform;
            particleContainer.transform.localPosition = Vector3.zero;

            for (int i = 0; i < particleCount; i++)
            {
                CreateParticle(particleContainer);
            }
        }

        /// <summary>
        /// Creates a single hologram particle.
        /// </summary>
        private void CreateParticle(GameObject container)
        {
            GameObject particle = new GameObject("HologramParticle");
            particle.transform.parent = container.transform;

            // Random position around the hologram
            Vector3 randomPos = Random.insideUnitSphere * 0.5f;
            particle.transform.localPosition = randomPos;

            // Create simple mesh
            MeshFilter filter = particle.AddComponent<MeshFilter>();
            filter.mesh = CreateParticleMesh();

            // Create renderer with hologram material
            MeshRenderer renderer = particle.AddComponent<MeshRenderer>();
            renderer.material = new Material(hologramMaterial);

            // Random scale
            float randomScale = Random.Range(0.02f, 0.08f);
            particle.transform.localScale = Vector3.one * randomScale;

            // Random lifetime
            Destroy(particle, Random.Range(particleLifetime * 0.5f, particleLifetime));
        }

        /// <summary>
        /// Creates a simple particle mesh.
        /// </summary>
        private Mesh CreateParticleMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, -0.5f, 0),
                new Vector3(0, 0.5f, 0)
            };
            mesh.triangles = new int[] { 0, 2, 1 };
            mesh.RecalculateNormals();
            return mesh;
        }

        /// <summary>
        /// Sets the rotation speed.
        /// </summary>
        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = speed;
        }

        /// <summary>
        /// Sets the hologram color.
        /// </summary>
        public void SetHologramColor(Color color)
        {
            hologramColor = color;
            if (hologramMaterial != null)
            {
                hologramMaterial.SetColor("_EmissionColor", hologramColor);
            }
        }

        /// <summary>
        /// Enables or disables the hologram effect.
        /// </summary>
        public void SetHologramEffect(bool enabled)
        {
            useHologramEffect = enabled;
            if (enabled)
            {
                ApplyHologramMaterial();
            }
            else
            {
                RestoreOriginalMaterials();
            }
        }

        /// <summary>
        /// Gets the current rotation angle.
        /// </summary>
        public float CurrentRotationAngle => currentRotationAngle;
    }
}
