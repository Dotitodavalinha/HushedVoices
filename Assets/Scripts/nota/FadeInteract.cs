using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInteract : MonoBehaviour //script similar al NoteInteract pero para q se auto desvanezca 
{
    public GameObject ShowThisObject;

    [SerializeField] private List<GameObject> outlinerCubes = new List<GameObject>(); //una lista de Outliners en caso de hacer falta 

    private bool InRange = false;
    private bool NoteIsOpen = false;

    private GameObject Player;

    [SerializeField] public GameObject PressE;
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    
    void Start()
    {
        PressE.SetActive(false);

        ShowThisObject.SetActive(false);
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
            PressE.SetActive(true);

        }
        if (!InRange)
        {
            PressE.SetActive(false);
        }

        if (NoteIsOpen)
        {
            ShowThisObject.SetActive(true);
            PressE.SetActive(false);
        }
        if (!NoteIsOpen)
        {
            ShowThisObject.SetActive(false);

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
            
        }

    }
}
