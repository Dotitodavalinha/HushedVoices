using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConcentrationUIController : MonoBehaviour
{
    [Header("Prefabs / Assets")]
    [Tooltip("Imagen para cada 'uso'. Puede ser el mismo prefab; se le baja alpha cuando el uso está gastado.")]
    [SerializeField] private Image useIconPrefab;
    [Tooltip("Sprite lleno para uso disponible (opcional).")]
    [SerializeField] private Sprite fullSprite;
    [Tooltip("Sprite vacío para uso gastado (opcional).")]
    [SerializeField] private Sprite emptySprite;

    [Tooltip("Contenedor horizontal para los usos (se crea si no existe).")]
    [SerializeField] private HorizontalLayoutGroup usesContainerPrefab;

    [Tooltip("Barra (Image fillAmount) que representa los segundos activos. Se muestra solo cuando está activo.")]
    [SerializeField] private Image durationBarPrefab;

    [Header("Layout")]
    [Tooltip("Margen desde el borde de la pantalla (px).")]
    [SerializeField] private Vector2 screenPadding = new Vector2(24, 24);
    [Tooltip("Lado de la pantalla para los usos.")]
    [SerializeField] private AnchorSide anchorSide = AnchorSide.Right;

    private Canvas _canvas;
    private RectTransform _root;
    private HorizontalLayoutGroup _usesContainer;
    private Image _durationBar;
    private readonly List<Image> _useIcons = new();

    private ConcentrationManager M => ConcentrationManager.Instance;

    public enum AnchorSide { Left, Right }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EnsureCanvas();
        BuildStaticUI();
        TrySubscribe();
    }

    private void OnEnable() => TrySubscribe();
    private void OnDisable() => Unsubscribe();

    private void Update()
    {
        if (M == null) return;

        // Actualizar barra de duración solo si está activa
        if (_durationBar != null)
        {
            bool active = M.IsActive();
            _durationBar.gameObject.SetActive(active);
            if (active)
            {
                float t = Mathf.Approximately(M.DurationSeconds, 0f) ? 0f : (M.RemainingSeconds / M.DurationSeconds);
                _durationBar.fillAmount = Mathf.Clamp01(t);
            }
        }
    }

    private void TrySubscribe()
    {
        if (M == null) return;
        // Estado inicial
        EnsureUsesBuilt();
        RefreshUses(M.UsesRemaining);

        // Eventos
        M.OnUsesChanged += RefreshUses;
        M.OnConcentrationStarted += HandleStarted;
        M.OnConcentrationEnded += HandleEnded;
    }

    private void Unsubscribe()
    {
        if (M == null) return;
        M.OnUsesChanged -= RefreshUses;
        M.OnConcentrationStarted -= HandleStarted;
        M.OnConcentrationEnded -= HandleEnded;
    }

    private void EnsureCanvas()
    {
        _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null)
        {
            var go = new GameObject("ConcentrationCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            _canvas = go.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1f; // prioriza alto

            DontDestroyOnLoad(go);
        }
        _root = _canvas.transform as RectTransform;
    }

    private void BuildStaticUI()
    {
        // Contenedor de usos (HorizontalLayoutGroup)
        if (_usesContainer == null)
        {
            if (usesContainerPrefab != null)
                _usesContainer = Instantiate(usesContainerPrefab, _root);
            else
            {
                var go = new GameObject("ConcentrationUses", typeof(RectTransform), typeof(HorizontalLayoutGroup));
                _usesContainer = go.GetComponent<HorizontalLayoutGroup>();
                _usesContainer.transform.SetParent(_root, false);
                _usesContainer.spacing = 8f;
                _usesContainer.childAlignment = TextAnchor.UpperRight;
                _usesContainer.childForceExpandHeight = false;
                _usesContainer.childForceExpandWidth = false;
            }

            var rt = _usesContainer.transform as RectTransform;
            rt.sizeDelta = new Vector2(200, 64);
            AnchorToSide(rt, anchorSide, screenPadding);
        }

        // Barra de duración (Image con fillAmount)
        if (_durationBar == null)
        {
            if (durationBarPrefab != null)
                _durationBar = Instantiate(durationBarPrefab, _root);
            else
            {
                var go = new GameObject("ConcentrationDurationBar", typeof(RectTransform), typeof(Image));
                _durationBar = go.GetComponent<Image>();
                _durationBar.type = Image.Type.Filled;
                _durationBar.fillMethod = Image.FillMethod.Horizontal;
                _durationBar.fillOrigin = (int)Image.OriginHorizontal.Left;
            }

            var rt = _durationBar.rectTransform;
            rt.sizeDelta = new Vector2(280, 18);
            // Colocamos la barra abajo del grupo de usos, mismo lado
            AnchorToSide(rt, anchorSide, screenPadding + new Vector2(0, 64));
            _durationBar.gameObject.SetActive(false);
        }
    }

    private void EnsureUsesBuilt()
    {
        if (M == null) return;

        int needed = M.MaxUsesPerDay;
        // Crear o reciclar
        while (_useIcons.Count < needed)
        {
            Image icon = null;
            if (useIconPrefab != null)
                icon = Instantiate(useIconPrefab, _usesContainer.transform);
            else
            {
                var go = new GameObject("UseIcon", typeof(RectTransform), typeof(Image));
                icon = go.GetComponent<Image>();
            }

            var rt = icon.rectTransform;
            rt.sizeDelta = new Vector2(24, 24);
            _useIcons.Add(icon);
        }
        // Si sobran, desactivar (no destruir para reusar)
        for (int i = 0; i < _useIcons.Count; i++)
            _useIcons[i].gameObject.SetActive(i < needed);

        RefreshUses(M.UsesRemaining);
    }

    private void RefreshUses(int usesRemaining)
    {
        if (_useIcons.Count == 0 && M != null) EnsureUsesBuilt();
        if (_useIcons.Count == 0) return;

        int total = Mathf.Min(_useIcons.Count, M != null ? M.MaxUsesPerDay : _useIcons.Count);
        for (int i = 0; i < total; i++)
        {
            bool available = i < usesRemaining;
            var img = _useIcons[i];

            if (fullSprite != null && emptySprite != null)
            {
                img.sprite = available ? fullSprite : emptySprite;
                img.color = Color.white;
            }
            else
            {
                // fallback: alpha
                img.sprite = img.sprite;
                img.color = new Color(1, 1, 1, available ? 1f : 0.25f);
            }
        }
    }

    private void HandleStarted()
    {
        if (_durationBar != null) _durationBar.gameObject.SetActive(true);
    }

    private void HandleEnded()
    {
        if (_durationBar != null)
        {
            _durationBar.fillAmount = 0f;
            _durationBar.gameObject.SetActive(false);
        }
        // También refrescamos usos por si algún flujo los alteró
        if (M != null) RefreshUses(M.UsesRemaining);
    }

    private static void AnchorToSide(RectTransform rt, AnchorSide side, Vector2 padding)
    {
        if (side == AnchorSide.Right)
        {
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.anchoredPosition = new Vector2(-padding.x, -padding.y);
        }
        else // Left
        {
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(padding.x, -padding.y);
        }
    }
}
