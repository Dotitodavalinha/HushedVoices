using UnityEngine;

public class MultiClueInteractable : UIInteractable
{
    [Header("Clue Settings")]
    [SerializeField] private string[] clueIDs; // Array para múltiples pistas
    [SerializeField] private bool showImportantAlert = false;

    private void Awake()
    {
        if (destroyAfterUse && AreAllCluesFound())
        {
            Deactivate();
        }
    }

    protected override void OnActivate()
    {
        if (destroyAfterUse && AreAllCluesFound())
        {
            Deactivate();
            return;
        }

        base.OnActivate();
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        SoundManager.instance.PlaySound(SoundID.CluePickupSound);

        if (PlayerClueTracker.Instance != null)
        {
            foreach (string id in clueIDs)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    PlayerClueTracker.Instance.AddClue(id);
                    Debug.Log($"Clue added: {id}");
                }
            }

            if (showImportantAlert)
            {
                Debug.Log($"Important clues collected!");
            }
        }
    }

    private bool AreAllCluesFound()
    {
        if (PlayerClueTracker.Instance == null) return false;

        foreach (string id in clueIDs)
        {
            if (!PlayerClueTracker.Instance.HasClue(id))
            {
                return false;
            }
        }
        return true;
    }
}