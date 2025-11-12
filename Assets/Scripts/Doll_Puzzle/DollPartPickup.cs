using UnityEngine;

public enum DollPartType { Head, Torso, ArmL, ArmR, LegL, LegR }

public class DollPartPickup : MonoBehaviour
{
    [Header("Config")]
    public DollPartType partType;
    [Tooltip("Si está en true, se recoge automáticamente al ACTIVAR el interactuable (E).")]
    public bool autoCollectOnFirstActivate = true;

    [Header("Refs (opcionales, se autodescubren)")]
    public UIInteractable uiInteractable;                    // para detectar activación y cerrarlo
    public RevealableByConcentration revealable;             // para apagar VFX/outline al recoger

    private bool collected;
    private bool lastIsOpen;

    void Reset()
    {
        uiInteractable = GetComponent<UIInteractable>();
        revealable = GetComponent<RevealableByConcentration>();
    }

    void Awake()
    {
        if (!uiInteractable) uiInteractable = GetComponent<UIInteractable>();
        if (!revealable) revealable = GetComponent<RevealableByConcentration>();
    }

    void Update()
    {
        if (!autoCollectOnFirstActivate || collected || uiInteractable == null) return;

        // Detecta flanco de apertura del interactuable (IsOpen true recién activado)
        bool isOpen = uiInteractable.IsOpen;
        if (isOpen && !lastIsOpen)
        {
            Collect();                // recoge en el momento que se abre
        }
        lastIsOpen = isOpen;
    }

   
    public void Collect()
    {
        if (collected) return;
        collected = true;

        print("avisamos al manager q agarramos pista");
        if (PuzzleManager.Instance != null)
            PuzzleManager.Instance.RegisterFoundPart(partType);
    }
}

