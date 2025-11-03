using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ConcentrationUIController : MonoBehaviour
{
    [Header("Sprites (Opcional)")]
    [Tooltip("Sprite lleno para uso disponible.")]
    [SerializeField] private Sprite fullSprite;
    [Tooltip("Sprite vacío para uso gastado.")]
    [SerializeField] private Sprite emptySprite;

    // ----- CAMPOS MODIFICADOS -----
    [Header("Referencias de UI (Asignar en Editor)")]
    [Tooltip("La Barra (Image) que muestra la duración. Debe ser de tipo 'Filled'.")]
    [SerializeField] private Image durationBar;

    [Tooltip("La lista de Imágenes (Image) que representan los 3 usos.")]
    [SerializeField] private List<Image> useIcons;
    // ----- FIN CAMPOS MODIFICADOS -----

    [Header("Ojos de concentración")]
    [SerializeField] private List<Animator> eyesAnimators;

    private Coroutine eyeRoutine;
    private int currentEyeIndex = 0;
    private bool isEyeSequenceActive = false;

    private Canvas _canvas;
    private RectTransform _root;
    // Se eliminan _usesContainer, _durationBar, y _useIcons (ahora son campos serializados)

    private ConcentrationManager M => ConcentrationManager.Instance;

    // Se elimina 'AnchorSide' enum

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        EnsureCanvas();
        // Se elimina BuildStaticUI()

        // Ocultamos la barra de duración al inicio
        if (durationBar != null)
        {
            durationBar.gameObject.SetActive(false);
        }

        TrySubscribe();
    }

    private void OnEnable() => TrySubscribe();
    private void OnDisable() => Unsubscribe();

    private void Update()
    {
        if (M == null) return;

        // Usamos la referencia 'durationBar' directamente
        if (durationBar != null)
        {
            bool active = M.IsActive();
            durationBar.gameObject.SetActive(active);
            if (active)
            {
                float t = Mathf.Approximately(M.DurationSeconds, 0f) ? 0f : (M.RemainingSeconds / M.DurationSeconds);
                durationBar.fillAmount = Mathf.Clamp01(t);
            }
        }
    }

    private void TrySubscribe()
    {
        if (M == null) return;

        EnsureUsesBuilt();
        RefreshUses(M.UsesRemaining);

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

    // MODIFICADO: Ahora solo busca un Canvas, no lo crea.
    private void EnsureCanvas()
    {
        _canvas = FindObjectOfType<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("ConcentrationUIController: ¡No se encontró ningún Canvas en la escena! La UI no funcionará.");
            return;
        }
        _root = _canvas.transform as RectTransform;
    }

    // ELIMINADO: El método BuildStaticUI() ya no es necesario,
    // ya que la UI se construye en el Editor.

    // MODIFICADO: Ya no instancia iconos, solo los activa/desactiva.
    private void EnsureUsesBuilt()
    {
        if (M == null || useIcons == null) return;

        int needed = M.MaxUsesPerDay;

        // En lugar de crear, activamos o desactivamos los iconos existentes
        for (int i = 0; i < useIcons.Count; i++)
        {
            if (useIcons[i] != null)
            {
                useIcons[i].gameObject.SetActive(i < needed);
            }
        }

        RefreshUses(M.UsesRemaining);
    }

    private void RefreshUses(int usesRemaining)
    {
        // Usamos la lista 'useIcons' asignada en el editor
        if (useIcons.Count == 0 && M != null) EnsureUsesBuilt();
        if (useIcons.Count == 0) return;

        if (M != null && usesRemaining == M.MaxUsesPerDay)
        {
            currentEyeIndex = 0;
            isEyeSequenceActive = false;

            foreach (Animator anim in eyesAnimators)
            {
                if (anim != null)
                    anim.gameObject.SetActive(false);
            }
        }

        int total = Mathf.Min(useIcons.Count, M != null ? M.MaxUsesPerDay : useIcons.Count);
        for (int i = 0; i < total; i++)
        {
            bool available = i < usesRemaining;
            var img = useIcons[i];

            if (img == null) continue; // Seguridad

            if (fullSprite != null && emptySprite != null)
            {
                img.sprite = available ? fullSprite : emptySprite;
                img.color = Color.white;
            }
            else
            {
                // No cambiamos el sprite, solo la opacidad
                img.color = new Color(1, 1, 1, available ? 1f : 0.25f);
            }
        }
    }

    // ELIMINADO: El método AnchorToSide() ya no es necesario.
    // El anclaje se hace en el RectTransform en el editor.

    private void HandleStarted()
    {
        if (isEyeSequenceActive) return;
        isEyeSequenceActive = true;

        if (durationBar != null) durationBar.gameObject.SetActive(true);

        if (eyeRoutine != null) StopCoroutine(eyeRoutine);

        if (currentEyeIndex < eyesAnimators.Count && eyesAnimators[currentEyeIndex] != null)
        {
            Animator currentAnimator = eyesAnimators[currentEyeIndex];
            currentAnimator.gameObject.SetActive(true);
            currentAnimator.Play("TurnOn");
        }
    }

    private void HandleEnded()
    {
        if (!isEyeSequenceActive) return;
        isEyeSequenceActive = false;

        if (durationBar != null)
        {
            durationBar.fillAmount = 0f;
            durationBar.gameObject.SetActive(false);
        }

        if (currentEyeIndex < eyesAnimators.Count && eyesAnimators[currentEyeIndex] != null)
        {
            eyeRoutine = StartCoroutine(TurnOffEyeSequence(eyesAnimators[currentEyeIndex]));
            currentEyeIndex++;
        }
    }

    private IEnumerator TurnOffEyeSequence(Animator anim)
    {
        anim.Play("TurnOff");
        yield return null;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length * (1f / anim.speed));
        anim.gameObject.SetActive(false);
    }
}