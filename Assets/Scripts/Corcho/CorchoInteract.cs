using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorchoInteract : MonoBehaviour
{
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] private ClueBoardManager corchoManager;
    [SerializeField] private FolioAnimation folio;

    [SerializeField] public GameObject PressE;
    [SerializeField] private bool UI_Activa = false;

    private void Start()
    {
        PressE.SetActive(false);
    }

    public void SetUIState(bool active)
    {
        UI_Activa = active;
    }

    private void Update()
    {
        if (UI_Activa)
        {
            PressE.SetActive(false);

            if (Input.GetKeyDown(KeyCode.E))
            {
                CerrarCorcho();
            }
            return;
        }

        if (zonaInteraccion.jugadorDentro)
        {
            PressE.SetActive(true);

            if (GameManager.Instance.BlockEInput)
                return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                AbrirCorcho();
            }
        }
        else
        {
            PressE.SetActive(false);
        }
    }

    private void AbrirCorcho()
    {
        if (!GameManager.Instance.TryLockUI())
            return;

        UI_Activa = true;
        PressE.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        corchoManager.OpenBoard();
        folio.OnUIReopened();
        FindObjectOfType<ExitUnlocker>()?.MarcarCorchoUsado();
    }

    private void CerrarCorcho()
    {
        UI_Activa = false;

        GameManager.Instance.UnlockUI();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        corchoManager.CloseBoard();
    }

    public void ForceUIState(bool active)
    {
        UI_Activa = active;
    }
}