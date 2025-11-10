using UnityEngine;
using System.Collections;

public class Tumbler : MonoBehaviour
{
    private Vector3 initialPosition;

    [SerializeField]
    private int tumblerNumber;

    [SerializeField]
    private Vector3 lockedOffset = new Vector3(0, 1.0f, 0.0f);

    public int TumblerNumber => tumblerNumber;

    private bool isLocked = false;
    public bool IsLocked => isLocked;

    void Start()
    {
        initialPosition = transform.position;
        isLocked = false;
    }

    public void LockTumbler()
    {
        transform.position = initialPosition + lockedOffset;
        isLocked = true;
    }

    public void ResetTumbler()
    {
        transform.position = initialPosition;
        isLocked = false;
    }

    // Corutina para 'Tanteo': Tiembla Y VUELVE (PISTA)
    public IEnumerator CorrectTumbler()
    {
        float duration = 1.0f;
        float elapsed = 0f;
        float magnitude = 0.1f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(0f, 1f) * magnitude;
            transform.position = initialPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = initialPosition;
    }
}