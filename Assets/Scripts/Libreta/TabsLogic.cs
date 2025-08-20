using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsLogic : MonoBehaviour
{
    public GameObject[] sections;
    public LibretaUI libretaUI; // ← arrastralo en el inspector
    public PageAnimation animationController;
    private int currentSectionIndex = -1;

    public void OpenSection(int index)
    {
        if (index < 0 || index >= sections.Length)
        {
            Debug.LogWarning("Índice de sección inválido: " + index);
            return;
        }

        libretaUI.AbrirLibretaDesdeBoton();

        if (index != currentSectionIndex)
        {
            animationController?.PlayAnimation();
            currentSectionIndex = index;
        }

        for (int i = 0; i < sections.Length; i++)
            sections[i].SetActive(i == index);

        SoundManager.instance.PlaySound(SoundID.PageTurnSound);
    }

    public void CloseAllSections()
    {
        foreach (GameObject section in sections)
        {
            section.SetActive(false);
        }
    }
}
