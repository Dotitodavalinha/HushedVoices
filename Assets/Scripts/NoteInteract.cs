using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class NoteInteract : MonoBehaviour
{

    public GameObject NoteImage;
    public float InteractRange = 5f;
    public Image NoteIcon;
    public Sprite spriteNoteDark;
    public Sprite spriteNoteColor;

    private bool DiscoverNote = false;
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
        NoteIcon.sprite = spriteNoteDark;
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
        if (!DiscoverNote)
        {
            NoteIcon.sprite = spriteNoteColor;
            DiscoverNote = true;
        }

        InRange = false;
        NoteIsOpen = true;
        StartCoroutine(Backtogame());
    }


    System.Collections.IEnumerator Backtogame()
    {
        yield return new WaitForSeconds(2.5f);
        NoteIsOpen = false;
    }
}

