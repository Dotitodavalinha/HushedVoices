using UnityEngine;

public class NPCMoodController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite normalFace;
    public Sprite happyFace;
    public Sprite angryFace;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform moodIconTransform;

    void Update()
    {
        if (playerTransform != null && moodIconTransform != null)
        {
            Vector3 toPlayer = playerTransform.position - moodIconTransform.position;
            toPlayer.y = 0; 
            moodIconTransform.forward = toPlayer.normalized;
        }
    }



    void Start()
    {
        SetMoodNormal(); 
    }

    public void SetMoodHappy()
    {
        spriteRenderer.sprite = happyFace;
    }

    public void SetMoodNormal()
    {
        spriteRenderer.sprite = normalFace;
    }

    public void SetMoodAngry()
    {
        spriteRenderer.sprite = angryFace;
    }
}
