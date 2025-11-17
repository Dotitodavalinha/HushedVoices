using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CluePanelActivator : MonoBehaviour
{
    [Tooltip("El objeto de la UI que contiene las pistas arrastrables (los ClueNodes).")]
    [SerializeField] private GameObject cluePanelUI;

    [SerializeField] private ExitUnlocker exitUnlocker;

    private void Awake()
    {
        if (exitUnlocker == null)
        {
            exitUnlocker = FindObjectOfType<ExitUnlocker>();
        }

        if (cluePanelUI != null && exitUnlocker != null)
        {
            cluePanelUI.SetActive(exitUnlocker.corchoEstaLimpio);
        }
    }

    private void OnEnable()
    {
        BrokenClueCleaner.OnAllBrokenCleaned += ActivateCluePanel;
    }

    private void OnDisable()
    {
        BrokenClueCleaner.OnAllBrokenCleaned -= ActivateCluePanel;
    }

    private void ActivateCluePanel()
    {
        if (cluePanelUI != null)
        {
            cluePanelUI.SetActive(true);
            Debug.Log("CluePanelActivator: Corcho limpio, UI de pistas arrastrables activada.");
        }
    }
}
