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

    private bool yaInteractuó = false;

    
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
                yaInteractuó = true;
                text.text = "";

                // Aquí podrías iniciar un diálogo real, si lo tuvieras
            }
        }
        else if (!zonaInteraccion.jugadorDentro)
        {
            yaInteractuó = false;
            text.text = "";
        }
    }
}
