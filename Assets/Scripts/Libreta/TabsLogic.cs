using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabsLogic : MonoBehaviour
{
    public GameObject[] sections;

    public void OpenSection(int index)
    {
        for (int i = 0; i < sections.Length; i++)
            sections[i].SetActive(i == index);
    }
}
