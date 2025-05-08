using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class NPCMoodController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite normalFace;
    public Sprite happyFace;
    public Sprite angryFace;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform moodIconTransform;

    //asigno policias y materiales
    public GameObject policias;


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

        //alambreee

        policias.SetActive(false);


    }

    public void SetMoodNormal()
    {
        spriteRenderer.sprite = normalFace;
    }

    public void SetMoodAngry()
    {
        spriteRenderer.sprite = angryFace;

        //alambreee
        policias.SetActive(true);

    }
}
