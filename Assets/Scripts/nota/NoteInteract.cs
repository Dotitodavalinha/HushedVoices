using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class NoteInteract : MonoBehaviour
{
    public GameObject NoteImage;

    [SerializeField] private List<GameObject> outlinerCubes = new List<GameObject>(); //una lista de Outliners en caso de hacer falta 

    [SerializeField] private CluePickup cluePickup;
    [SerializeField] private CluePickupByList cluePickupByList;

    private bool InRange = false;
    private bool NoteIsOpen = false;

    private GameObject Player;

    [SerializeField] public GameObject text;
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] private bool IsImportantClue = false;

    [SerializeField] private bool isTV = false;

    void Start()
    {
        text.SetActive(false);
        if (NoteImage != null)
        {
            NoteImage.SetActive(false);
        }
        Player = GameObject.FindWithTag("Player");


    }

    void Update()
    {
        float distancia = Vector3.Distance(Player.transform.position, transform.position);

        if (zonaInteraccion.jugadorDentro)
        {

            InRange = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                ShowNote();
            }
        }
        else if (!zonaInteraccion.jugadorDentro)
        {

            InRange = false;
        }
        if (InRange && NoteIsOpen == false)
        {
            text.SetActive(true);

        }
        if (!InRange)
        {
            text.SetActive(false);
        }

        if (NoteIsOpen)
        {
            if (NoteImage != null)
            {
                Debug.LogWarning("no hay ningun game object asignado a NoteInteract(no tiene pq estar mal)");
                NoteImage.SetActive(true);
            }
            text.SetActive(false);
        }
        if (!NoteIsOpen)
        {
            if (NoteImage != null)
            {
                NoteImage.SetActive(false);
            }
        }

    }

    void ShowNote()
    {
        NoteIsOpen = !NoteIsOpen;

        if (outlinerCubes != null && outlinerCubes.Count > 0)
        {
            foreach (var cube in outlinerCubes)
            {
                if (cube != null)
                    cube.SetActive(false);
            }
        }
        else
        {
            Transform singleOutliner = transform.Find("OutlinerCube");
            if (singleOutliner != null)
                singleOutliner.gameObject.SetActive(false);
        }


        if (NoteIsOpen)
        {
            if (!GameManager.Instance.TryLockUI())
            {
                NoteIsOpen = false; // cancela apertura
                return;
            }
        }
        else
        {
            GameManager.Instance.UnlockUI();
            if (isTV)
            {
                ExitUnlocker exitUnlocker = FindObjectOfType<ExitUnlocker>();
                if (exitUnlocker != null)
                {
                  //  exitUnlocker.MarcarTVLeida();
                }
            }
            else if (IsImportantClue)
            {

                if (cluePickup != null)
                {
                    cluePickup.PickUpClue(); // agrego la nueva IDclue
                }

                if (cluePickupByList != null)
                {
                    cluePickupByList.PickUpClues();
                }

            }
        }

    }
}

