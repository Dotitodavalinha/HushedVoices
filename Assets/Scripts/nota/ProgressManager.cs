
using UnityEngine;
using System;
using System.Collections.Generic;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    public bool BensNoteUnlocked;
    public bool CoffeeShopUnlocked;
    public bool Policez;
    public bool ChangeDialoguePolicez;
    public bool Policeznt;
    public bool ColegioStreet;




    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CambiarRootNPC(string npcID, string rootName)
    {
        var npcList = FindObjectsOfType<NPCDialogue>();
        foreach (var npc in npcList)
        {
            if (npc.npcName == npcID)
            {
                Debug.Log("cambia el root de " + npcID + " a " + rootName);
                npc.GoToRoot(rootName);
                break;
            }
        }
    }

}

