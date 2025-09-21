using UnityEngine;

public class CluePickup : MonoBehaviour
{
    [SerializeField] private string clueID;
    [SerializeField] private bool destroyOnPickup;
    [SerializeField] private bool ShowAlertOnPickup;

    private void Start()
    {
        if (destroyOnPickup)
        {
            if (PlayerClueTracker.Instance != null && PlayerClueTracker.Instance.HasClue(clueID))
            {
                Destroy(gameObject); // o gameObject.SetActive(false) 
            }
        }

    }

    public void PickUpClue()
    {
        if (!PlayerClueTracker.Instance.HasClue(clueID))
        {
            if (ShowAlertOnPickup)
            {
                SoundManager.instance.PlaySound(SoundID.CluePickupSound);
                ImportantClue.Instance.ShowClueAlert(); // si es una pista importante popeamos la alerta
            }
            SoundManager.instance.PlaySound(SoundID.CluePickupSound);
            PlayerClueTracker.Instance.AddClue(clueID);
            Debug.Log("Se registró la pista: " + clueID);
        }

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
