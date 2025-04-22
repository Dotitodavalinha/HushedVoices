using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class NoteInteract : MonoBehaviour
{
    public GameObject InteractButton; 
    public GameObject NoteImage; 
    public float InteractRange = 5f;
    public Image NoteIcon;
    public Sprite spriteNoteDark;
    public Sprite spriteNoteColor;

    private bool DiscoverNote = false;
    private bool InRange = false;
    private GameObject Player; 

    void Start()
    {
        InteractButton.SetActive(false);
        NoteImage.SetActive(false); 
        Player = GameObject.FindWithTag("Player");
        NoteIcon.sprite = spriteNoteDark;
    }

    void Update()
    {
        if (Vector3.Distance(Player.transform.position, transform.position) <= InteractRange)
        {
            if (!InRange)
            {
                InRange = true;
                InteractButton.SetActive(true);
            }
        }
        else
        {
            if (InRange)
            {
                InRange = false;
                InteractButton.SetActive(false);
            }
        }

        if (InRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowNote();
        }
    }

    void ShowNote()
    {
        if(!DiscoverNote)
        {
            NoteIcon.sprite = spriteNoteColor;
            DiscoverNote = true;
        }
        InteractButton.SetActive(false); 
        NoteImage.SetActive(true); 
        StartCoroutine(Backtogame()); 
    }

    System.Collections.IEnumerator Backtogame()
    {
        yield return new WaitForSeconds(2f);
        NoteImage.SetActive(false);
    }
}

