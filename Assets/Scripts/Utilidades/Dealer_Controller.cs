using UnityEngine;

public class Dealer_Controller : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;

    [Header("Debug / Override")]
    public bool forceSleepOverride = false; // editable desde el editor

    [Header("Sleep Anchor (posición)")]
    [SerializeField] private Transform sleepAnchor;
    [SerializeField] private float standingYOffset = 0f; // cuánto sube en Y al estar parado
    [SerializeField] private float standingZOffset = 0f; // cuánto mueve en Z al estar parado

    private Vector3 originalAnchorLocalPos;
    private bool hasSleepAnchor = false;

    private static readonly int IsSleepingHash = Animator.StringToHash("IsSleeping");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError($"[Dealer_Controller] No encontré Animator en {name}");

        // Cacheo posición original
        if (sleepAnchor != null)
        {
            hasSleepAnchor = true;
            originalAnchorLocalPos = sleepAnchor.localPosition;
        }
    }

    private void Start()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Si instanció en Park Sleep
        forceSleepOverride = currentScene == "park";

        SetSleeping(forceSleepOverride);
    }

    private void Update()
    {
        // Permite ver los cambios en tiempo real si tocás el bool desde el editor
        SetSleeping(forceSleepOverride);
        //ProgressManager.Instance.CambiarRootNPC("???", "Root_Dealer");
    }

    public void SetSleeping(bool isSleeping)
    {
        if (animator != null)
            animator.SetBool(IsSleepingHash, isSleeping);

        // --- NUEVO: cambiar la Root del NPC según su estado ---
        if (ProgressManager.Instance != null)
        {
            if (isSleeping)
            {
                ProgressManager.Instance.CambiarRootNPC("???", "Root_Vagabundo");
            }
            else
            {
                ProgressManager.Instance.CambiarRootNPC("???", "Root_Dealer");
            }
        }
        // -----------------------------------------------------

        if (!hasSleepAnchor) return;

        if (isSleeping)
        {
            // Sleep = posición original
            sleepAnchor.localPosition = originalAnchorLocalPos;
        }
        else
        {
            // Idle/Dealer = posición original + offset Y/Z
            sleepAnchor.localPosition = new Vector3(
                originalAnchorLocalPos.x,
                originalAnchorLocalPos.y + standingYOffset,
                originalAnchorLocalPos.z + standingZOffset
            );
        }
    }


    public void Sleep() => SetSleeping(true);
    public void WakeUp() => SetSleeping(false);
}
