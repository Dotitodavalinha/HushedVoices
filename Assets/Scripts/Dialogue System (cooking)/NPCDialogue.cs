using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
   
    public string npcName;
    public List<DialogueSO> roots; // aca pasas todos los dialogos posibles q va a tener(roots) dsp llamas a ProgressManager.Instance.CambiarRootNPC("npcname", "RootName"); y cambiara de dialogo !
    [SerializeField]public DialogueSO currentRoot;

    public NPCMoodController moodController;
    public NightManager NightManager;

    private void Start()
    {
        //buscar el night manager em escena
    }

    public void GoToRoot(string rootName)
    {
        var root = roots.Find(r => r.name == rootName);
        if (root != null)
            currentRoot = root; Debug.Log("Se cambio el root excitosamente");
    }


    public void StartDialogue(DialogueSO dialogue)
    {
        if(npcName == "Police" || npcName == "Policez")
        {
            NightManager.TalkingToPolice();
        }
        DialogueManager.Instance.StartDialogue(dialogue, this);
    }

}
