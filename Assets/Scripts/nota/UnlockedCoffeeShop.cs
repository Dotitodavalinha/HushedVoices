
using UnityEngine;

public class UnlockedCoffeeShop : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        ProgressManager.Instance.CoffeeShopUnlocked = true;
        ProgressManager.Instance.CambiarRootNPC("MARINA", "RootPolice0");

    }

}
