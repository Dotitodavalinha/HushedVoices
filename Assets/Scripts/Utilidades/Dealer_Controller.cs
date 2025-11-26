using UnityEngine;

public class Dealer_Controller : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator; // opcional, se puede arrastrar en el inspector

    [Header("Debug / Override")]
    public bool forceSleepOverride = false;   // editable desde el editor

    private static readonly int IsSleepingHash = Animator.StringToHash("IsSleeping");

    private void Awake()
    {
        // Si no asignaste el Animator a mano, lo busca en este objeto o hijos.
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError($"[Dealer_Controller] No encontré Animator en {name}");
        }
    }

    private void Start()
    {
        // Averiguo la escena actual
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Si estoy en "Park" => Sleep. Si no => Idle.
        forceSleepOverride = currentScene == "park";

        // Aplicar estado inicial
        SetSleeping(forceSleepOverride);
    }

    private void Update()
    {
        // Si cambiás el bool en el editor durante Play, se refleja al instante.
        SetSleeping(forceSleepOverride);
    }

    /// <summary>
    /// Setea directamente el bool isSleeping en el Animator.
    /// </summary>
    public void SetSleeping(bool isSleeping)
    {
        if (animator == null) return;
        animator.SetBool(IsSleepingHash, isSleeping);
    }

    public void Sleep()
    {
        SetSleeping(true);
    }

    public void WakeUp()
    {
        SetSleeping(false);
    }
}
