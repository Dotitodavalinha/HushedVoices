using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Rendering.Universal;

public class NPCMoodController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite normalFace;
    public Sprite happyFace;
    public Sprite angryFace;

    
    [SerializeField] private Transform moodIconTransform;

    //asigno policias y materiales
    public GameObject policias;
    public Material paranoia;
    public Material ambient;

    void Update()
    {
        if (ActiveCameraTracker.ActiveCamera != null && moodIconTransform != null)
        {
            Vector3 toCam = ActiveCameraTracker.ActiveCamera.transform.position - moodIconTransform.position;
            toCam.y = 0;
            moodIconTransform.forward = toCam.normalized;
        }
    }




    void Start()
    {

        SetMoodNormal();

        //alambreee

        paranoia.SetInt("_vig_amount", 0);
        ambient.SetFloat("dayNight", 1f);

        policias.SetActive(false);
    }

    public void SetMoodHappy()
    {

        spriteRenderer.sprite = happyFace;

        //alambreee

        paranoia.SetInt("_vig_amount", 0);
        ambient.SetFloat("dayNight", 1f);

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

        paranoia.SetInt("_vig_amount", 1);
        ambient.SetFloat("dayNight", 0f);
        policias.SetActive(true);

    }
}
