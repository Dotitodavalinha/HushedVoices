using UnityEngine;

public class CluePickup : MonoBehaviour
{
    [SerializeField] private string clueID;
    [SerializeField] private bool destroyOnPickup = true;

    public void PickUpClue()
    {
        if (!PlayerClueTracker.Instance.HasClue(clueID))
        {
            PlayerClueTracker.Instance.AddClue(clueID);
            Debug.Log("Se registró la pista: " + clueID);

            if (destroyOnPickup)
                Destroy(gameObject);
        }
    }
}
