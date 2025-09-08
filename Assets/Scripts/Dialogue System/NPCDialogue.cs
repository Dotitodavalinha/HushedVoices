using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
   
    public string npcName;
    public int npcVoiceType;
    public List<DialogueSO> roots; // aca pasas todos los dialogos posibles q va a tener(roots) dsp llamas a ProgressManager.Instance.CambiarRootNPC("npcname", "RootName"); y cambiara de dialogo !
    [SerializeField]public DialogueSO currentRoot;

    public NPCMoodController moodController;
    public NightManager NightManager;
    public bool noRotateToLook = true;
    private bool isChangingRoot = false;
    private void Start()
    {
        NightManager = GameObject.Find("NightManager")?.GetComponent<NightManager>();
        string savedRoot = ProgressManager.Instance.GetCurrentRoot(npcName, currentRoot != null ? currentRoot.name : "");
        if (!string.IsNullOrEmpty(savedRoot))
        {
            SetRoot(savedRoot, false); // false = no llamar a ProgressManager otra vez
        }

    }
    public void SetRoot(string rootName, bool updateProgressManager = true)
    {
        if (isChangingRoot) return; // evita recursión

        var root = roots.Find(r => r.name == rootName);
        if (root == null)
        {
            Debug.LogWarning($"No se encontró root '{rootName}' en {npcName}");
            return;
        }

        if (currentRoot != null && currentRoot.name == rootName)
        {
            // Ya es el root actual, nada que hacer
            return;
        }

        isChangingRoot = true;
        currentRoot = root;

        if (updateProgressManager)
        {
            ProgressManager.Instance.CambiarRootNPC(npcName, rootName);
        }

        isChangingRoot = false;

        Debug.Log($"Se cambió el root de {npcName} a {rootName}");
    }
    public void GoToRoot(string rootName, bool updateProgressManager = true)
    {
        var root = roots.Find(r => r.name == rootName);
        if (root == null)
        {
            Debug.LogWarning($"No se encontró root '{rootName}' en {npcName}");
            return;
        }

        if (currentRoot != null && currentRoot.name == rootName)
            return; // ya es el root actual

        currentRoot = root;

        Debug.Log($"Se cambió el root de {npcName} a {rootName}");

        if (updateProgressManager)
        {
            ProgressManager.Instance.CambiarRootNPC(npcName, rootName);
        }
    }



    public void StartDialogue(DialogueSO dialogue)
    {
        if(npcName == "Police" || npcName == "PoliceZ")
        {
            Debug.Log("le estas hablando a un gorra");
            NightManager.TalkingToPolice();
        }
        DialogueManager.Instance.StartDialogue(dialogue, this);
    }

}
