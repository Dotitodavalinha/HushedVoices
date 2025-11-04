using UnityEngine;

public class ClueRecoveryStation : MonoBehaviour
{
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerClueTracker.Instance.lostClues.Count > 0)
            {
                PlayerClueTracker.Instance.RecoverAllClues();

                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("No hay pistas para recuperar.");
            }
        }
    }
}