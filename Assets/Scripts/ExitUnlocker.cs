using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUnlocker : MonoBehaviour
{
    [Header("Collider de salida (puerta, trigger, etc.)")]
    [SerializeField] private GameObject exitCollider;

    private bool corchoUsado = false;

    private void Start()
    {
        if (exitCollider != null)
            exitCollider.SetActive(false);
    }

    private void Update()
    {
        if (HasClues() && corchoUsado)
        {
            if (exitCollider != null)
            {
                exitCollider.SetActive(true);
                Debug.Log(" Salida desbloqueada");
            }
        }
    }

    private bool HasClues()
    {
        return PlayerClueTracker.Instance.HasClue("list") &&
               PlayerClueTracker.Instance.HasClue("bensNote");
    }

    public void MarcarCorchoUsado()
    {
        corchoUsado = true;
        Debug.Log("Corcho utilizado");
    }
}
