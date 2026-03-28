using UnityEngine;

namespace Alperen.Scripts.Environment
{
    /// <summary>
    /// Manages the atmospheric conditions of the cave environment.
    /// Controls lighting, fog, ambient settings for optimal flashlight exploration.
    /// </summary>
    public class AtmosphereManager : MonoBehaviour
    {
        [Header("Lighting Settings")]
        [SerializeField] private bool useCustomLighting = true;
        [SerializeField] private float ambientIntensity = 0.05f;
        [SerializeField] private float reflectionIntensity = 0.1f;
        [SerializeField] private Color ambientColor = new Color(0.02f, 0.02f, 0.03f, 1f);

        [Header("Fog Settings")]
        [SerializeField] private bool useFog = true;
        [SerializeField] private FogMode fogMode = FogMode.Exponential;
        [SerializeField] private float fogDensity = 0.05f;
        [SerializeField] private Color fogColor = new Color(0.01f, 0.01f, 0.015f, 1f);
        [SerializeField] private float fogStartDistance = 0f;
        [SerializeField] private float fogEndDistance = 30f;

        [Header("Environment Lights")]
        [SerializeField] private Light mainDirectionalLight;
        [SerializeField] private float directionalLightIntensity = 0.1f;
        [SerializeField] private Color directionalLightColor = new Color(0.2f, 0.2f, 0.25f, 1f);

        [Header("Point Lights (Ambient Glow)")]
        [SerializeField] private bool useAmbientPointLights = true;
        [SerializeField] private int pointLightCount = 3;
        [SerializeField] private float pointLightIntensity = 0.5f;
        [SerializeField] private Color pointLightColor = new Color(0.3f, 0.2f, 0.1f, 1f);
        [SerializeField] private float pointLightRange = 8f;
        [SerializeField] private Vector3 pointLightArea = new Vector3(15f, 5f, 15f);

        [Header("Cave Atmosphere")]
        [SerializeField] private bool enableCaveAtmosphere = true;
        [SerializeField] private float lightProbeIntensity = 0.1f;
        [SerializeField] private bool updateInRealtime = false;

        private Light[] ambientLights;
        private float originalAmbientIntensity;
        private Color originalAmbientColor;
        private float originalReflectionIntensity;
        private Color originalFogColor;
        private float originalFogDensity;

        private void Awake()
        {
            // Store original settings
            originalAmbientIntensity = RenderSettings.ambientIntensity;
            originalAmbientColor = RenderSettings.ambientLight;
            originalReflectionIntensity = RenderSettings.reflectionIntensity;
            originalFogColor = RenderSettings.fogColor;
            originalFogDensity = RenderSettings.fogDensity;

            // Find main directional light if not assigned
            if (mainDirectionalLight == null)
            {
                mainDirectionalLight = FindObjectOfType<Light>();
                if (mainDirectionalLight != null && mainDirectionalLight.type == LightType.Directional)
                {
                    mainDirectionalLight = FindObjectOfType<Light>();
                }
            }
        }

        private void Start()
        {
            ApplyAtmosphereSettings();
        }

        private void Update()
        {
            if (updateInRealtime)
            {
                ApplyAtmosphereSettings();
            }
        }

        /// <summary>
        /// Applies all atmosphere settings to the scene.
        /// </summary>
        public void ApplyAtmosphereSettings()
        {
            SetLightingSettings();
            SetFogSettings();
            SetDirectionalLight();
            CreateAmbientPointLights();
        }

        /// <summary>
        /// Sets the ambient lighting for a dark cave environment.
        /// </summary>
        private void SetLightingSettings()
        {
            if (useCustomLighting)
            {
                RenderSettings.ambientIntensity = ambientIntensity;
                RenderSettings.ambientLight = ambientColor;
                RenderSettings.reflectionIntensity = reflectionIntensity;

                Debug.Log($"AtmosphereManager: Ambient ışık ayarlandı (Intensity: {ambientIntensity})");
            }
        }

        /// <summary>
        /// Sets the fog settings for atmospheric depth.
        /// </summary>
        private void SetFogSettings()
        {
            if (useFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = fogMode;
                RenderSettings.fogDensity = fogDensity;
                RenderSettings.fogColor = fogColor;

                if (fogMode == FogMode.Linear)
                {
                    RenderSettings.fogStartDistance = fogStartDistance;
                    RenderSettings.fogEndDistance = fogEndDistance;
                }

                Debug.Log($"AtmosphereManager: Sis ayarlandı (Density: {fogDensity})");
            }
            else
            {
                RenderSettings.fog = false;
            }
        }

        /// <summary>
        /// Adjusts the main directional light for cave environment.
        /// </summary>
        private void SetDirectionalLight()
        {
            if (mainDirectionalLight != null)
            {
                mainDirectionalLight.intensity = directionalLightIntensity;
                mainDirectionalLight.color = directionalLightColor;

                // Disable shadows for better performance in VR
                mainDirectionalLight.shadows = LightShadows.None;

                Debug.Log($"AtmosphereManager: Ana ışık ayarlandı (Intensity: {directionalLightIntensity})");
            }
        }

        /// <summary>
        /// Creates ambient point lights for subtle cave glow.
        /// </summary>
        private void CreateAmbientPointLights()
        {
            // Remove old ambient lights first
            if (ambientLights != null)
            {
                foreach (var light in ambientLights)
                {
                    if (light != null)
                    {
                        Destroy(light.gameObject);
                    }
                }
            }

            if (useAmbientPointLights && enableCaveAtmosphere)
            {
                ambientLights = new Light[pointLightCount];

                for (int i = 0; i < pointLightCount; i++)
                {
                    GameObject lightObj = new GameObject($"AmbientPointLight_{i}");
                    lightObj.transform.parent = transform;

                    // Random position within the area
                    Vector3 randomPos = new Vector3(
                        Random.Range(-pointLightArea.x * 0.5f, pointLightArea.x * 0.5f),
                        Random.Range(-pointLightArea.y * 0.5f, pointLightArea.y * 0.5f),
                        Random.Range(-pointLightArea.z * 0.5f, pointLightArea.z * 0.5f)
                    );
                    lightObj.transform.localPosition = randomPos;

                    Light light = lightObj.AddComponent<Light>();
                    light.type = LightType.Point;
                    light.intensity = pointLightIntensity;
                    light.color = pointLightColor;
                    light.range = pointLightRange;
                    light.shadows = LightShadows.None;

                    ambientLights[i] = light;
                }

                Debug.Log($"AtmosphereManager: {pointLightCount} tane ambient ışık oluşturuldu");
            }
        }

        /// <summary>
        /// Restores the original Unity render settings.
        /// </summary>
        public void RestoreOriginalSettings()
        {
            RenderSettings.ambientIntensity = originalAmbientIntensity;
            RenderSettings.ambientLight = originalAmbientColor;
            RenderSettings.reflectionIntensity = originalReflectionIntensity;
            RenderSettings.fogColor = originalFogColor;
            RenderSettings.fogDensity = originalFogDensity;

            if (mainDirectionalLight != null)
            {
                mainDirectionalLight.intensity = originalAmbientIntensity;
            }

            Debug.Log("AtmosphereManager: Orijinal ayarlar geri yüklendi");
        }

        /// <summary>
        /// Gradually changes the ambient intensity over time.
        /// </summary>
        public void LerpAmbientIntensity(float targetIntensity, float duration)
        {
            StartCoroutine(LerpAmbientCoroutine(targetIntensity, duration));
        }

        private System.Collections.IEnumerator LerpAmbientCoroutine(float targetIntensity, float duration)
        {
            float startIntensity = RenderSettings.ambientIntensity;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                RenderSettings.ambientIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                yield return null;
            }

            RenderSettings.ambientIntensity = targetIntensity;
        }

        /// <summary>
        /// Toggles the fog on/off.
        /// </summary>
        public void ToggleFog(bool enable)
        {
            useFog = enable;
            SetFogSettings();
        }

        /// <summary>
        /// Sets a custom fog density.
        /// </summary>
        public void SetFogDensity(float density)
        {
            fogDensity = Mathf.Max(0f, density);
            SetFogSettings();
        }

        /// <summary>
        /// Sets the ambient intensity directly.
        /// </summary>
        public void SetAmbientIntensity(float intensity)
        {
            ambientIntensity = Mathf.Max(0f, intensity);
            SetLightingSettings();
        }

        /// <summary>
        /// Gets the current ambient intensity.
        /// </summary>
        public float CurrentAmbientIntensity => RenderSettings.ambientIntensity;

        /// <summary>
        /// Checks if fog is currently enabled.
        /// </summary>
        public bool IsFogEnabled => RenderSettings.fog;

        private void OnDestroy()
        {
            // Clean up ambient lights
            if (ambientLights != null)
            {
                foreach (var light in ambientLights)
                {
                    if (light != null)
                    {
                        Destroy(light.gameObject);
                    }
                }
            }
        }
    }
}
