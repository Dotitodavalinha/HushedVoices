using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class NoteInteract : MonoBehaviour
{

    public GameObject NoteImage;
    public float InteractRange = 5f;
   
    //public Sprite spriteNoteDark;
    //public Sprite spriteNoteColor;

    [SerializeField]private PlayerMovementLocker playerLocker;

    private bool InRange = false;
    private bool NoteIsOpen = false;

    private GameObject Player;

    [SerializeField] public TMP_Text text;
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    void Start()
    {
        text.text = " ";
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
            text.text = "Press 'E'";
        }
        if (!InRange)
        {
            text.text = "";
        }

        if (NoteIsOpen)
        {
            NoteImage.SetActive(true);
        }
        if (!NoteIsOpen)
        {
            NoteImage.SetActive(false);
        }

    }


    void ShowNote()
    {
        if (!ProgressManager.Instance.BensNoteUnlocked)
        {
            ProgressManager.Instance.BensNoteUnlocked = true;
            Debug.Log("Desbloqueaste la nota de Ben");
        }

        NoteIsOpen = !NoteIsOpen;

        if (NoteIsOpen)
            playerLocker.LockMovement();
        else
            playerLocker.UnlockMovement();
    
    }
}

