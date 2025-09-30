using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeGoToStreet : MonoBehaviour
{
    [Header("Referencia al ExitUnlocker")]
    public ExitUnlocker exitUnlocker;

    [Header("Mensajes faltantes")]
    public GameObject Board;   // Mensaje de que falta usar el corcho
    public GameObject Sleep;   // Mensaje de que falta dormir
    public GameObject Clues;   // Mensaje de que faltan pistas
    public GameObject Cleaning; // Mensaje de que falta limpiar

    private void Start()
    {
        DesactivarTodos();
    }

    public void RevisarEstado()
    {
        // Siempre apagamos todo antes de activar uno solo
        DesactivarTodos();

        // Si ya completó todo, no hay nada que mostrar
        if (exitUnlocker.boardUsed && exitUnlocker.hasSlept &&
            exitUnlocker.houseCleaned && TieneClues())
        {
            return;
        }

        // Caso en que aún falte algo
        if (!exitUnlocker.boardUsed)
        {
            Board.SetActive(true);
        }
        else if (!exitUnlocker.hasSlept)
        {
            Sleep.SetActive(true);
        }
        else if (!exitUnlocker.houseCleaned)
        {
            Cleaning.SetActive(true);
        }
        else if (!TieneClues())
        {
            Clues.SetActive(true);
        }
    }

    private void DesactivarTodos()
    {
        Board.SetActive(false);
        Sleep.SetActive(false);
        Clues.SetActive(false);
        Cleaning.SetActive(false);
    }

    private bool TieneClues()
    {
        return PlayerClueTracker.Instance.HasClue("list") &&
               PlayerClueTracker.Instance.HasClue("bensNote") &&
               PlayerClueTracker.Instance.HasClue("Tv");
    }
}
