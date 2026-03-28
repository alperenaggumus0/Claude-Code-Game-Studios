using UnityEngine;
using TMPro;
using Alperen.Scripts.Interaction;

namespace Alperen.Scripts.UI
{
    /// <summary>
    /// Manages the info tablet display.
    /// Shows dinosaur information and 3D hologram when a bone is socketed.
    /// </summary>
    public class TabletManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject tabletCanvas;
        [SerializeField] private GameObject contentPanel;
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private GameObject hologramContainer;
        [SerializeField] private Transform hologramModel;

        [Header("Hologram Settings")]
        [SerializeField] private bool rotateHologram = true;
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private float hologramScale = 1f;

        [Header("Display Settings")]
        [SerializeField] private bool showTabletOnStart = false;
        [SerializeField] private float fadeSpeed = 0.5f;
        [SerializeField] private Vector3 displayPositionOffset = new Vector3(0, 0.15f, 0.3f);
        [SerializeField] private Vector3 displayRotation = new Vector3(15, 0, 0);

        [Header("BoneSocket Connection")]
        [SerializeField] private BoneSocketSystem boneSocketSystem;

        [Header("Events")]
        [SerializeField] private UnityEngine.Events.UnityEvent onTabletShow;
        [SerializeField] private UnityEngine.Events.UnityEvent onTabletHide;
        [SerializeField] private UnityEngine.Events.UnityEvent onContentUpdate;

        private bool isTabletVisible = false;
        private float currentAlpha = 0f;
        private CanvasGroup canvasGroup;
        private MeshRenderer hologramRenderer;

        /// <summary>
        /// Event raised when the tablet is shown.
        /// </summary>
        public event System.Action OnTabletShow;

        /// <summary>
        /// Event raised when the tablet is hidden.
        /// </summary>
        public event System.Action OnTabletHide;

        /// <summary>
        /// Event raised when content is updated.
        /// </summary>
        public event System.Action<DinosaurInfo> OnContentUpdate;

        private void Awake()
        {
            // Setup CanvasGroup for fading
            if (tabletCanvas != null)
            {
                canvasGroup = tabletCanvas.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = tabletCanvas.AddComponent<CanvasGroup>();
                }
            }

            // Get hologram renderer
            if (hologramContainer != null)
            {
                hologramRenderer = hologramContainer.GetComponentInChildren<MeshRenderer>();
            }

            // Find BoneSocketSystem if not assigned
            if (boneSocketSystem == null)
            {
                boneSocketSystem = FindObjectOfType<BoneSocketSystem>();
                if (boneSocketSystem != null)
                {
                    Debug.Log($"TabletManager: BoneSocketSystem otomatik bulundu: {boneSocketSystem.name}");
                }
            }
        }

        private void Start()
        {
            // Initial visibility state
            SetTabletVisibility(showTabletOnStart);

            // Subscribe to bone socket events
            if (boneSocketSystem != null)
            {
                boneSocketSystem.OnBoneSocketed += OnBoneSocketed;
            }
            else
            {
                Debug.LogWarning("TabletManager: BoneSocketSystem atanmadı! Kemik bilgisi gösterilemeyecek.");
            }
        }

        private void Update()
        {
            // Rotate hologram if visible
            if (isTabletVisible && rotateHologram && hologramModel != null)
            {
                hologramModel.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            if (boneSocketSystem != null)
            {
                boneSocketSystem.OnBoneSocketed -= OnBoneSocketed;
            }
        }

        /// <summary>
        /// Called when a bone is socketed.
        /// </summary>
        private void OnBoneSocketed(GameObject bone)
        {
            // Show the tablet
            ShowTablet();

            // Get dinosaur info from bone name
            DinosaurInfo info = GetDinosaurInfo(bone.name);

            // Update the content
            UpdateContent(info);

            Debug.Log($"TabletManager: Kemik bilgisi gösteriliyor - {info.name}");
        }

        /// <summary>
        /// Gets dinosaur information based on bone name.
        /// </summary>
        private DinosaurInfo GetDinosaurInfo(string boneName)
        {
            // Simple lookup based on bone name
            // In a real project, this would be data-driven
            switch (boneName.ToLower())
            {
                case "dinobone":
                    return new DinosaurInfo
                    {
                        name = "Tyrannosaurus Rex",
                        period = "Kretase (68-66 Milyon Yıl Önce)",
                        description = "T-Rex, karada yaşamış en büyük etçil dinozorlardan biridir. Güçlü çeneleri ve keskin dişleri ile avını kolayca parçalayabilmiştir.",
                        weight = "7-8 Ton",
                        height = "4-4.5 Metre",
                        length = "12-13 Metre"
                    };
                case "stegosaurusbone":
                    return new DinosaurInfo
                    {
                        name = "Stegosaurus",
                        period = "Jura (155-150 Milyon Yıl Önce)",
                        description = "Stegosaurus, sırtında büyük plakalar ve kuyruğunda dört sivri uç ile tanınan otçul bir dinozordur.",
                        weight = "5-7 Ton",
                        height = "4 Metre",
                        length = "9 Metre"
                    };
                case "triceratopsbone":
                    return new DinosaurInfo
                    {
                        name = "Triceratops",
                        period = "Kretase (68-66 Milyon Yıl Önce)",
                        description = "Triceratops, üç boynuzlu kalkanlı bir dinozordur. Güçlü boynuzları ve kalkanı ile kendini savunabilmiştir.",
                        weight = "6-12 Ton",
                        height = "3 Metre",
                        length = "9 Metre"
                    };
                default:
                    return new DinosaurInfo
                    {
                        name = "Bilinmeyen Dinozor",
                        period = "Bilinmiyor",
                        description = "Bu kemik hakkında daha fazla bilgi için araştırma yapılıyor.",
                        weight = "Bilinmiyor",
                        height = "Bilinmiyor",
                        length = "Bilinmiyor"
                    };
            }
        }

        /// <summary>
        /// Shows the tablet with fade-in effect.
        /// </summary>
        public void ShowTablet()
        {
            SetTabletVisibility(true);
        }

        /// <summary>
        /// Hides the tablet with fade-out effect.
        /// </summary>
        public void HideTablet()
        {
            SetTabletVisibility(false);
        }

        /// <summary>
        /// Sets the tablet visibility.
        /// </summary>
        private void SetTabletVisibility(bool visible)
        {
            isTabletVisible = visible;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;
            }

            if (contentPanel != null)
            {
                contentPanel.SetActive(visible);
            }

            // Update hologram visibility
            if (hologramRenderer != null)
            {
                if (hologramRenderer.material != null)
                {
                    Color color = hologramRenderer.material.color;
                    color.a = visible ? 0.8f : 0f;
                    hologramRenderer.material.color = color;
                }
            }

            // Trigger events
            if (visible)
            {
                onTabletShow?.Invoke();
                OnTabletShow?.Invoke();
            }
            else
            {
                onTabletHide?.Invoke();
                OnTabletHide?.Invoke();
            }
        }

        /// <summary>
        /// Updates the tablet content with dinosaur information.
        /// </summary>
        public void UpdateContent(DinosaurInfo info)
        {
            if (infoText != null)
            {
                infoText.text = $"<b>İSİM:</b> {info.name}\n\n" +
                                $"<b>DÖNEM:</b> {info.period}\n\n" +
                                $"<b>AÇIKLAMA:</b> {info.description}\n\n" +
                                $"<b>AĞIRLIK:</b> {info.weight}\n" +
                                $"<b>BOY:</b> {info.height}\n" +
                                $"<b>UZUNLUK:</b> {info.length}";
            }

            // Update hologram scale based on dinosaur size
            if (hologramContainer != null)
            {
                float targetScale = GetHologramScale(info.name);
                hologramContainer.transform.localScale = Vector3.one * targetScale;
            }

            // Trigger events
            onContentUpdate?.Invoke();
            OnContentUpdate?.Invoke(info);
        }

        /// <summary>
        /// Gets the hologram scale based on dinosaur name.
        /// </summary>
        private float GetHologramScale(string dinosaurName)
        {
            // Scale based on dinosaur size
            if (dinosaurName.Contains("Tyrannosaurus") || dinosaurName.Contains("T-Rex"))
            {
                return hologramScale * 1.5f;
            }
            else if (dinosaurName.Contains("Stegosaurus") || dinosaurName.Contains("Triceratops"))
            {
                return hologramScale * 1.2f;
            }
            return hologramScale;
        }

        /// <summary>
        /// Manually sets the dinosaur info.
        /// </summary>
        public void SetDinosaurInfo(DinosaurInfo info)
        {
            ShowTablet();
            UpdateContent(info);
        }

        /// <summary>
        /// Toggles the tablet visibility.
        /// </summary>
        public void ToggleTablet()
        {
            SetTabletVisibility(!isTabletVisible);
        }

        /// <summary>
        /// Checks if the tablet is currently visible.
        /// </summary>
        public bool IsVisible => isTabletVisible;

        /// <summary>
        /// Gets the current dinosaur info being displayed.
        /// </summary>
        public DinosaurInfo CurrentInfo { get; private set; }
    }

    /// <summary>
    /// Data structure for dinosaur information.
    /// </summary>
    [System.Serializable]
    public class DinosaurInfo
    {
        public string name;
        public string period;
        public string description;
        public string weight;
        public string height;
        public string length;
    }
}
