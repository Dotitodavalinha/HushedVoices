
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
    public string PolicemanZDialogueRoot = "RootPoliceZ1";
    private Dictionary<string, string> npcRoots = new Dictionary<string, string>();




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

    public string GetCurrentRoot(string npcName, string defaultRoot)
    {
        if (npcRoots.ContainsKey(npcName))
            return npcRoots[npcName];
        return defaultRoot; // si nunca se cambió, devolvemos el inicial
    }

    public void ResetAllBools()
    {
        var campos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var campo in campos)
        {
            if (campo.FieldType == typeof(bool))
            {
                campo.SetValue(this, false);
                //Debug.Log($"Se apaga {campo.Name}");
            }
        }

        //Debug.Log("Todos los bools del ProgressManager fueron reseteados a false.");
    }
    public void ResetNPCRoots()
    {
        var npcList = FindObjectsOfType<NPCDialogue>();
        foreach (var npc in npcList)
        {
            if (npc.roots != null && npc.roots.Count > 0)
            {
                var firstRoot = npc.roots[0];

                // Verificar que el primer elemento de la lista no sea nulo
                if (firstRoot != null)
                {
                    string defaultRootName = firstRoot.name;

                    npc.GoToRoot(defaultRootName, false);
                    npcRoots[npc.npcName] = defaultRootName;
                    Debug.Log($"Se reinició el root de {npc.npcName} a {defaultRootName}");
                }
                else
                {
                    Debug.LogWarning($"El primer root de NPC '{npc.npcName}' es nulo en la escena actual.");
                }
            }
            else
            {
                Debug.LogWarning($"El NPC '{npc.npcName}' no tiene raíces (roots) asignadas en el Inspector de esta escena.");
            }
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
                npc.GoToRoot(rootName, false); // <--- NO llamamos otra vez al ProgressManager
                break;
            }
        }

        // Guardamos en diccionario para persistencia
        npcRoots[npcID] = rootName;
    }

}

