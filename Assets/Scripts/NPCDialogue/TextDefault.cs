using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDefault : MonoBehaviour
{
    [SerializeField] public NPCInteractionZone zonaInteraccion;
    [SerializeField] public TMP_Text text;
    [SerializeField] public Transform npcTransform;     
    [SerializeField] public Transform playerTransform;

    
    public Cinemachine.CinemachineFreeLook freeLookCam;

    private bool yaInteractuó = false;
    public event System.Action OnDialogueStart;
    public event System.Action OnDialogueEnd;


    public bool IsInteracting => yaInteractuó;

    private void Start()
    {
        text.gameObject.SetActive(true);
        text.text = "";
    }

    private void Update()
    {
        if (zonaInteraccion.jugadorDentro && !yaInteractuó)
        {
            text.text = "Press 'E' to talk";

            if (Input.GetKeyDown(KeyCode.E))
            {
                freeLookCam.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                yaInteractuó = true;
                text.text = "";

                OnDialogueStart?.Invoke(); 
            }

        }
        else if (!zonaInteraccion.jugadorDentro)
        {
            if (yaInteractuó)
                OnDialogueEnd?.Invoke(); 

            yaInteractuó = false;
            text.text = "";

        }
    }
}
