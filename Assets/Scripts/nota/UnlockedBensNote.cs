using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockedBensNote : MonoBehaviour
{
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    private void Update()
    {
        if (zonaInteraccion.jugadorDentro)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!ProgressManager.Instance.BensNoteUnlocked)
                {
                    ProgressManager.Instance.BensNoteUnlocked = true;
                    Debug.Log("Desbloqueaste la nota de Ben");
                }
            }
        }
    }
}
