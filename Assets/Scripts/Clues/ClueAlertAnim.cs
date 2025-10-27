using UnityEngine;

public class ClueAlertAnim : MonoBehaviour
{
    public void OnAnimationFinished()
    {
        Destroy(gameObject);
    }
}
