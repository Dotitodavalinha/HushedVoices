using UnityEngine;
using System.Collections.Generic;

public class CluePickupByList : MonoBehaviour
{
    [SerializeField] private List<string> clueIDs;
    [SerializeField] private bool destroyOnPickup;
    [SerializeField] private bool showAlertOnPickup;

    public void PickUpClues()
    {
        bool anyAdded = false;

        foreach (var clueID in clueIDs)
        {
            if (!PlayerClueTracker.Instance.HasClue(clueID))
            {
                PlayerClueTracker.Instance.AddClue(clueID);
                Debug.Log("Se registró la pista: " + clueID);
                anyAdded = true;
            }
        }

        if (anyAdded && showAlertOnPickup)
        {
            ImportantClue.Instance.ShowClueAlert();
        }

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
