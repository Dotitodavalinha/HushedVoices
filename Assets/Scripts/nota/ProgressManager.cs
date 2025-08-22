
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    public bool BensNoteUnlocked;
    public bool CoffeeShopUnlocked;
    public bool Policez;
    public bool ChangeDialoguePolicez;
    public bool Policeznt;
    public bool ColegioStreet;
    public bool GotCoffe;
    public bool LostCoffe;




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

    public void ResetAllBools()
    {
        var campos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var campo in campos)
        {
            if (campo.FieldType == typeof(bool))
            {
                campo.SetValue(this, false);
                Debug.Log($"Se apaga {campo.Name}");
            }
        }

        Debug.Log("Todos los bools del ProgressManager fueron reseteados a false.");
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

