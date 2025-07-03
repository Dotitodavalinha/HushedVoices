using UnityEngine;

public class CluePickup : MonoBehaviour
{
    [SerializeField] private string clueID;
    [SerializeField] private bool destroyOnPickup;
    [SerializeField] private bool ShowAlertOnPickup;

    public void PickUpClue()
    {
        if (!PlayerClueTracker.Instance.HasClue(clueID))
        {
            if (ShowAlertOnPickup)
            {
                ImportantClue.Instance.ShowClueAlert(); // si es una pista importante popeamos la alerta
            }

            PlayerClueTracker.Instance.AddClue(clueID);
            Debug.Log("Se registró la pista: " + clueID);
        }

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
