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
            PlayerClueTracker.OnClueAdded += CheckAndUnlockExit;
            //Debug.Log(" ExitUnlocker se suscribió al evento de pistas.");
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
            PlayerClueTracker.OnClueAdded += CheckAndUnlockExit;

        BrokenClueCleaner.OnAllBrokenCleaned -= MarcarCorchoLimpio;
        BrokenClueCleaner.OnAllBrokenCleaned += MarcarCorchoLimpio;
    }

    private void CheckAndUnlockExit()
    {
        if (HasClues() && boardUsed && hasSlept && houseCleaned && corchoEstaLimpio)
        {
            //Debug.Log(" Todas las condiciones cumplidas — desbloqueando salida.");

            if (exitCollider != null)
                exitCollider.SetActive(true);

            if (OulinePuerta != null)
                OulinePuerta.SetActive(true);
        }
        else
        {
            //Debug.Log($"Clues: {HasClues()}, Board: {boardUsed}, Sleep: {hasSlept}, Clean: {houseCleaned}, Corcho limpio: {corchoEstaLimpio}");
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
            PlayerClueTracker.OnClueAdded -= CheckAndUnlockExit;
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
        CheckAndUnlockExit();
    }

    private void MarcarComoDormido()
    {
        hasSlept = true;
        CheckAndUnlockExit();
    }

    public void MarcarCorchoUsado()
    {
        boardUsed = true;
        CheckAndUnlockExit();
    }
    private void MarcarCasaLimpia()
    {
        houseCleaned = true;
        CheckAndUnlockExit();
    }

}