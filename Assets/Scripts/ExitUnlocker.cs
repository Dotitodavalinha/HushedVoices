using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUnlocker : MonoBehaviour
{
    [Header("Collider de salida (puerta, trigger, etc.)")]
    [SerializeField] private GameObject exitCollider;

    [SerializeField] private GameObject OulinePuerta;

    public bool boardUsed = false;
    public bool hasSlept = false;
    public bool houseCleaned = false;
    public bool corchoEstaLimpio = false;

    private void OnEnable()
    {
        SuscribirEventos();
    }

    private void Start()
    {
        SuscribirEventos();

        if (CleanManager.Instance != null)
        {
            CleanManager.Instance.OnHouseCleaned += MarcarCasaLimpia;
        }
        BedPass.OnPlayerSlept += MarcarComoDormido;

        if (PlayerClueTracker.Instance != null)
        {
            // MODIFICADO: Se suscribe al nuevo evento "OnCluesAdded" (plural)
            PlayerClueTracker.OnCluesAdded += CheckAndUnlockExit;
        }
        else
        {
            Debug.LogError(" No se encontró la instancia de PlayerClueTracker.");
        }


        if (exitCollider != null)
            exitCollider.SetActive(false);

        if (OulinePuerta != null)
            OulinePuerta.SetActive(false);
    }

    private void SuscribirEventos()
    {
        if (CleanManager.Instance != null)
            CleanManager.Instance.OnHouseCleaned += MarcarCasaLimpia;

        BedPass.OnPlayerSlept += MarcarComoDormido;

        if (PlayerClueTracker.Instance != null)
            // MODIFICADO: Se suscribe al nuevo evento "OnCluesAdded" (plural)
            PlayerClueTracker.OnCluesAdded += CheckAndUnlockExit;

        BrokenClueCleaner.OnAllBrokenCleaned -= MarcarCorchoLimpio;
        BrokenClueCleaner.OnAllBrokenCleaned += MarcarCorchoLimpio;
    }

    // MODIFICADO: La función ahora acepta "List<string> clues" para coincidir con el evento
    private void CheckAndUnlockExit(List<string> clues)
    {
        // No necesitamos usar la lista "clues" aquí, pero la necesitamos para que la firma coincida
        CheckConditions();
    }

    // Función separada para poder llamarla desde otros sitios sin el parámetro
    private void CheckConditions()
    {
        if (HasClues() && boardUsed && hasSlept && houseCleaned && corchoEstaLimpio)
        {
            if (exitCollider != null)
                exitCollider.SetActive(true);

            if (OulinePuerta != null)
                OulinePuerta.SetActive(true);
        }
    }


    private void OnDisable()
    {
        BedPass.OnPlayerSlept -= MarcarComoDormido;

        if (CleanManager.Instance != null)
        {
            CleanManager.Instance.OnHouseCleaned -= MarcarCasaLimpia;
        }

        if (PlayerClueTracker.Instance != null)
        {
            // MODIFICADO: Se desuscribe del nuevo evento "OnCluesAdded" (plural)
            PlayerClueTracker.OnCluesAdded -= CheckAndUnlockExit;
        }

        BrokenClueCleaner.OnAllBrokenCleaned -= MarcarCorchoLimpio;
    }

    private bool HasClues()
    {
        return PlayerClueTracker.Instance.HasClue("reportUI") &&
               PlayerClueTracker.Instance.HasClue("bensNote") &&
               PlayerClueTracker.Instance.HasClue("Tv");
    }

    public void MarcarCorchoLimpio()
    {
        corchoEstaLimpio = true;
        boardUsed = true;
        Debug.Log(" -> ESTADO ACTUALIZADO: Corcho limpio y usado.");
        CheckConditions(); // MODIFICADO
    }

    private void MarcarComoDormido()
    {
        hasSlept = true;
        CheckConditions(); // MODIFICADO
    }

    public void MarcarCorchoUsado()
    {
        boardUsed = true;
        CheckConditions(); // MODIFICADO
    }
    private void MarcarCasaLimpia()
    {
        houseCleaned = true;
        CheckConditions(); // MODIFICADO
    }
}