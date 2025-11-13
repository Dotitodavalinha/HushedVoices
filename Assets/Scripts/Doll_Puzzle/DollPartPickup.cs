using UnityEngine;

public enum DollPartType { Head, Torso, ArmL, ArmR, LegL, LegR }

public class DollPartPickup : MonoBehaviour
{
    [Header("Config")]
    public DollPartType partType;
    [Tooltip("Si está en true, se recoge automáticamente al ACTIVAR el interactuable (E).")]
    public bool autoCollectOnFirstActivate = true;

    [Header("Refs (opcionales, se autodescubren)")]
    [Tooltip("Interactuable que se usa SIN concentración")]
    public UIInteractable uiInteractableNormal;

    [Tooltip("Interactuable que se usa DURANTE la concentración")]
    public UIInteractable uiInteractableConcentration;

    public RevealableByConcentration revealable;             // para apagar VFX/outline al recoger

    private bool collected;
    private bool lastIsOpenNormal;
    private bool lastIsOpenConcentration;

    void Reset()
    {
        // Por si seguís usando un único UIInteractable en el mismo GO
        if (!uiInteractableNormal)
            uiInteractableNormal = GetComponent<UIInteractable>();

        if (!revealable)
            revealable = GetComponent<RevealableByConcentration>();
    }

    void Awake()
    {
        if (!uiInteractableNormal)
            uiInteractableNormal = GetComponent<UIInteractable>();

        if (!revealable)
            revealable = GetComponent<RevealableByConcentration>();
    }


    void Update()
    {
        if (!autoCollectOnFirstActivate || collected) return;

        // Interactuable normal (sin concentración)
        if (uiInteractableNormal != null)
        {
            bool isOpenNormal = uiInteractableNormal.IsOpen;
            if (isOpenNormal && !lastIsOpenNormal)
            {
                Collect();
            }
            lastIsOpenNormal = isOpenNormal;
        }

        // Interactuable de concentración
        if (uiInteractableConcentration != null)
        {
            bool isOpenConc = uiInteractableConcentration.IsOpen;
            if (isOpenConc && !lastIsOpenConcentration)
            {
                Collect();
            }
            lastIsOpenConcentration = isOpenConc;
        }
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

