using UnityEngine;
using UnityEngine.UI;

public class ClueCases : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject case1Panel;
    public GameObject case2Panel;
    public GameObject case3Panel;
    public GameObject case4Panel;


    private void ShowPanel(GameObject panelToShow)
    {
        mainMenuPanel.SetActive(false);
        case1Panel.SetActive(false);
        case2Panel.SetActive(false);
        case3Panel.SetActive(false);
        case4Panel.SetActive(false);

        panelToShow.SetActive(true);
    }

    public void OpenCase1()
    {
        ShowPanel(case1Panel);
    }

    public void OpenCase2()
    {
        ShowPanel(case2Panel);
    }

    public void OpenCase3()
    {
        ShowPanel(case3Panel);
    }

    public void OpenCase4()
    {
        ShowPanel(case4Panel);
    }

    public void OpenMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }
}