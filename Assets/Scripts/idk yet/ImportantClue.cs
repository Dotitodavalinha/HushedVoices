using UnityEngine;

public class ImportantClue : MonoBehaviour
{
    public static ImportantClue Instance;

    [SerializeField] private GameObject clueAlertPrefab;
    [SerializeField] private Transform canvasTransform;

    void Awake()
    {
        Instance = this;
    }

    public void ShowClueAlert()
    {
        Debug.LogWarning("ALERTA PISTA IMPORTANTE");
        GameObject alert = Instantiate(clueAlertPrefab, canvasTransform);
        alert.transform.SetAsLastSibling();
    }
}

