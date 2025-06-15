using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsLogic : MonoBehaviour
{
    public GameObject[] sections;
    public LibretaUI libretaUI; // ← arrastralo en el inspector
    

    public void OpenSection(int index)
    {
        Debug.Log("abriendo el index numero " + index);

        libretaUI.AbrirLibretaDesdeBoton(); // ← asegurate que la libreta esté abierta

        for (int i = 0; i < sections.Length; i++)
            sections[i].SetActive(i == index);
    }

     public void CloseAllSections()
    {
        foreach (GameObject section in sections)
        {
            section.SetActive(false);
        }
    }
}
