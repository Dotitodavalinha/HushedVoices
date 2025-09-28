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


    private void Start()
    {
        if (CleanManager.Instance != null)
        {
            CleanManager.Instance.OnHouseCleaned += MarcarCasaLimpia;
        }
        BedPass.OnPlayerSlept += MarcarComoDormido;

        if (PlayerClueTracker.Instance != null)
        {
            PlayerClueTracker.OnClueAdded += CheckAndUnlockExit;
            Debug.Log(" ExitUnlocker se suscribió al evento de pistas.");
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

    private void CheckAndUnlockExit()
    {
        if (HasClues() && boardUsed && hasSlept && houseCleaned )
        {
            if (exitCollider != null)
            {
                exitCollider.SetActive(true);
            }
            if (OulinePuerta != null)
            {
                OulinePuerta.SetActive(true);
            }
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
    }

    private bool HasClues()
    {
        return PlayerClueTracker.Instance.HasClue("list") &&
               PlayerClueTracker.Instance.HasClue("bensNote") &&
               PlayerClueTracker.Instance.HasClue("Tv");
    }

    private void MarcarComoDormido()
    {
        hasSlept = true;
        //Debug.Log("El jugador ha dormido.");
        CheckAndUnlockExit();
    }

    public void MarcarCorchoUsado()
    {
        boardUsed = true;
        //Debug.Log("Corcho utilizado.");
        CheckAndUnlockExit();
    }
    private void MarcarCasaLimpia()
    {
        houseCleaned = true;
        //Debug.Log(" El jugador limpio");
        CheckAndUnlockExit();
    }

}
