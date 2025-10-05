using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeGoToStreet : MonoBehaviour
{
    [Header("Referencia al ExitUnlocker")]
    public ExitUnlocker exitUnlocker;

    [Header("Mensajes faltantes")]
    public GameObject Board;    // Mensaje de que falta usar el corcho
    public GameObject Sleep;    // Mensaje de que falta dormir
    public GameObject Clues;    // Mensaje de que faltan pistas
    public GameObject Cleaning; // Mensaje de que falta limpiar

    private void Start()
    {
        DesactivarTodos();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RevisarEstado();
        }
    }

    public void RevisarEstado()
    {
        DesactivarTodos();

        bool tieneBoard = exitUnlocker.boardUsed;
        bool tieneSleep = exitUnlocker.hasSlept;
        bool tieneCleaning = exitUnlocker.houseCleaned;
        bool tieneClues = TieneClues();
        bool corchoLimpio = EstaCorchoLimpio();

        // Si ya completó todo, no hay nada que mostrar
        if (tieneBoard && tieneSleep && tieneCleaning && tieneClues && corchoLimpio)
        {
            Debug.Log("El jugador completó todas las tareas, puede salir.");
            return;
        }


        // Si NO completó nada, no mostrar nada
        int tareasCompletadas = 0;
        if (tieneBoard) tareasCompletadas++;
        if (tieneSleep) tareasCompletadas++;
        if (tieneCleaning) tareasCompletadas++;
        if (tieneClues) tareasCompletadas++;
        if (corchoLimpio) tareasCompletadas++;

        if (tareasCompletadas == 0)
        {
            Debug.Log("El jugador no hizo ninguna tarea aún, no mostrar mensajes.");
            return;
        }
        // ---------------------------

        // Caso en que haya hecho algo pero le falte otra cosa
        if (!tieneBoard)
        {
            Board.SetActive(true);
        }
        else if (!tieneSleep)
        {
            Sleep.SetActive(true);
        }
        else if (!tieneCleaning)
        {
            Cleaning.SetActive(true);
        }
        else if (!tieneClues)
        {
            Clues.SetActive(true);
        }
        else if (!corchoLimpio)
        {
            Cleaning.SetActive(true);
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
    private bool EstaCorchoLimpio()
    {
        BrokenClueCleaner[] rotas = FindObjectsOfType<BrokenClueCleaner>(true);
        return rotas.Length == 0;
    }
}
