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
            if (libretaAbierta)
            {
                CerrarLibreta();
            }
            else
            {
                AbrirLibreta(0);
                SoundManager.instance.PlaySound(SoundID.BookOpenSound);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!libretaAbierta)
            {
                AbrirLibreta(3);
            }
            else
            {
                CerrarLibreta();
            }
        }
    }

    void AbrirLibreta(int seccionInicial)
    {
        if (!GameManager.Instance.TryLockUI())
            return;

        libretaAbierta = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ActualizarUI();
        tabsLogic.OpenSection(seccionInicial);
    }

    void CerrarLibreta()
    {
        GameManager.Instance.UnlockUI();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        libretaAbierta = false;
        tabsLogic.CloseAllSections();

        tabsLogic.animationController?.StopAndHide();
    }

    public void AbrirLibretaDesdeBoton()
    {
        if (!libretaAbierta)
            AbrirLibreta(0);
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
