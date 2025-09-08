using UnityEngine;

public class UnlockableObject : MonoBehaviour
{
    public string objectID; // opcional si querés manejar varios

    private void Start()
    {
        if (ProgressManager.Instance != null && ProgressManager.Instance.ColegioStreet)
        {
            gameObject.SetActive(false);
        }
    }
}