using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCLightRepairUI : MonoBehaviour
{
    [Header("Referencias Visuales")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject sliderContainer;

    [Header("Configuración de Textos")]
    [SerializeField] private string textGoing = "Who turned off the lights?!";
    [SerializeField] private string textRepairing = "Fixing the fuse box...";
    [SerializeField] private string textReturning = "Finally, the lights are back.";

    [SerializeField] private NPCStoreKeeper npc;

    private NPCStoreKeeper.NPCRepairState currentState;

    private void OnEnable()
    {
        if (npc != null)
        {
            npc.OnTravelProgressChanged += UpdateSlider;
            npc.OnNPCStateChanged += UpdateStatusText;
        }
    }

    private void OnDisable()
    {
        if (npc != null)
        {
            npc.OnTravelProgressChanged -= UpdateSlider;
            npc.OnNPCStateChanged -= UpdateStatusText;
        }
    }

    private void UpdateStatusText(NPCStoreKeeper.NPCRepairState newState)
    {
        currentState = newState;

        if (statusText == null) return;

        switch (newState)
        {
            case NPCStoreKeeper.NPCRepairState.Going:
                statusText.text = textGoing;

                if (sliderContainer != null) sliderContainer.SetActive(true);
                break;
            case NPCStoreKeeper.NPCRepairState.Repairing:
                statusText.text = textRepairing;
                break;
            case NPCStoreKeeper.NPCRepairState.Returning:
                statusText.text = textReturning;
                break;
        }
    }

    private void UpdateSlider(float progress)
    {
        if (sliderContainer != null && !sliderContainer.activeSelf)
        {
            sliderContainer.SetActive(true);
        }

        progressSlider.value = progress;

        // Ocultar solo si está volviendo y la barra llegó a 0
        if (currentState == NPCStoreKeeper.NPCRepairState.Returning && progress <= 0.01f)
        {
            if (sliderContainer != null) sliderContainer.SetActive(false);
        }
    }
}