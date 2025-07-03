
using UnityEngine;

public class UnlockedCoffeeShop : MonoBehaviour
{
    [SerializeField] private string clueID;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        {
            ProgressManager.Instance.CoffeeShopUnlocked = true;
        }

        if (clueID != null)
        {
            PlayerClueTracker.Instance.AddClue(clueID);
        }

    }

}
