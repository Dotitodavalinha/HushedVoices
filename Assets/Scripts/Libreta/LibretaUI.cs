using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibretaUI : MonoBehaviour
{
    [SerializeField] private TabsLogic tabsLogic;

    [SerializeField] private GameObject notaBenUI;
    [SerializeField] private GameObject cafeteriaUI;
    [SerializeField] private GameObject PoliciazUI;
    [SerializeField] private GameObject PolicezntUI;
    [SerializeField] private GameObject GotCoffe;
    [SerializeField] private GameObject LostCoffe;

    private bool libretaAbierta = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            libretaAbierta = !libretaAbierta;

            if (libretaAbierta)
            {
                if (!GameManager.Instance.TryLockUI())
                {
                    libretaAbierta = false; // Cancela apertura
                    return;
                }

                // Ahora que sabemos que sí se abre, mostramos cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                ActualizarUI();
                tabsLogic.OpenSection(0);
                SoundManager.instance.PlaySound(SoundID.BookOpenSound);
            }
            else
            {
                GameManager.Instance.UnlockUI();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                tabsLogic.CloseAllSections();
            }
        }

    }

    public void AbrirLibretaDesdeBoton()
    {
        if (!libretaAbierta)
        {
            if (!GameManager.Instance.TryLockUI())
                return;

            libretaAbierta = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ActualizarUI();
        }

    }

    void ActualizarUI()
    {
        notaBenUI.SetActive(ProgressManager.Instance.BensNoteUnlocked);
        cafeteriaUI.SetActive(ProgressManager.Instance.CoffeeShopUnlocked);
        PoliciazUI.SetActive(ProgressManager.Instance.Policez);
        PolicezntUI.SetActive(ProgressManager.Instance.Policeznt);
        GotCoffe.SetActive(ProgressManager.Instance.GotCoffe);
        LostCoffe.SetActive(ProgressManager.Instance.LostCoffe);
    }
}
