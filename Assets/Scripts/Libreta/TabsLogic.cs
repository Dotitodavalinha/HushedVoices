using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsLogic : MonoBehaviour
{
    public GameObject[] sections;
    public LibretaUI libretaUI;
    public PageAnimation animationController;

    private int currentSectionIndex = -1;
    private int nextSectionIndex = -1;
    //private bool firstOpen = true;

    public void OpenSection(int index)
    {
        if (index < 0 || index >= sections.Length)
            return;

        libretaUI.AbrirLibretaDesdeBoton();

        if (currentSectionIndex < 0)
        {
            currentSectionIndex = index;
            for (int i = 0; i < sections.Length; i++)
                sections[i].SetActive(i == index);

            SoundManager.instance.PlaySound(SoundID.PageTurnSound);
            return;
        }

        if (index == currentSectionIndex)
            return;

        if (animationController != null && !animationController.gameObject.activeSelf)
            animationController.gameObject.SetActive(true);

        nextSectionIndex = index;
        animationController?.PlayAnimation();
    }

    public void ActivateNextSection()
    {
        if (nextSectionIndex >= 0 && nextSectionIndex < sections.Length)
        {
            for (int i = 0; i < sections.Length; i++)
                sections[i].SetActive(i == nextSectionIndex);

            SoundManager.instance.PlaySound(SoundID.PageTurnSound);

            currentSectionIndex = nextSectionIndex;
            nextSectionIndex = -1;
        }
    }

    public void CloseAllSections()
    {
        foreach (GameObject section in sections)
            section.SetActive(false);

        //firstOpen = true;
    }

    public void ResetCurrentSection()
    {
        currentSectionIndex = -1;
        nextSectionIndex = -1;
    }
}
