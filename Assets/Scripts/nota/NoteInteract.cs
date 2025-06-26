using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class NoteInteract : MonoBehaviour
{
    public GameObject NoteImage;

    [SerializeField] private PlayerMovementLocker playerLocker;

    private bool InRange = false;
    private bool NoteIsOpen = false;

    private GameObject Player;

    [SerializeField] public GameObject text;
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] private bool IsImportantClue = false;
    void Start()
    {
        text.SetActive(false);

        NoteImage.SetActive(false);
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
            NoteImage.SetActive(true);
            text.SetActive(false);
        }
        if (!NoteIsOpen)
        {
            NoteImage.SetActive(false);

        }

    }

    void ShowNote()
    {
        NoteIsOpen = !NoteIsOpen;

        transform.Find("OutlinerCube").gameObject.SetActive(false);

        if (NoteIsOpen)
            playerLocker.LockMovement();
        else
        {
            playerLocker.UnlockMovement();
            if (IsImportantClue)
            {
                ImportantClue.Instance.ShowClueAlert(); // si es una pista importante popeamos la alerta

            }
        }

    }
}

