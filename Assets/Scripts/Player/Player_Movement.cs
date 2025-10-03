using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float RunMagnitude = 1.5f;
    public float cameraFollowSpeed = 5f;
    public float cameraDistance = 5f;
    public float cameraHeight = 3f;
    public float rotationSpeed = 5f;
    float finalSpeed = 0f;
    public float footstepInterval = 0.7f;
    private float footstepTimer = 0f;



    private CharacterController controller;
    private Camera currentCam;
    private Camera lastCam;
    private bool holdingMovementInput;
    private bool isStreetScene;
    private bool isRoomInicioScene;
    private bool isRoomScene;
    public Animator anim;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
        currentCam = Camera.main;
        isRoomScene = SceneManager.GetActiveScene().name == "Room";
        isRoomInicioScene = SceneManager.GetActiveScene().name == "RoomInicio";
    }

    void Update()
    {

        float normalizedSpeed = 0f;


        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        holdingMovementInput = input.magnitude >= 0.1f;


        if (!isStreetScene && !holdingMovementInput && Camera.main != currentCam)
        {
            currentCam = Camera.main;
        }

        if (holdingMovementInput)
        {
            lastCam = currentCam;
        }

        Vector3 moveDir = Vector3.zero;

        if (isRoomScene || isRoomInicioScene)
        {
            //moveDir = transform.forward * input.z + transform.right * input.x;
            moveDir = Vector3.forward * -input.x + Vector3.right * input.z;
            
            //Debug.Log("is Street scene");
        }
        
        else
        {
            moveDir = Vector3.forward * input.z + Vector3.right * input.x;
            /*
            Camera camToUse = lastCam != null ? lastCam : currentCam;
            Vector3 camForward = camToUse.transform.forward;
            Vector3 camRight = camToUse.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDir = camForward * input.z + camRight * input.x;*/
        }

        if (holdingMovementInput)
        {

            float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? RunMagnitude : 1f;
            finalSpeed = moveSpeed * speedMultiplier;
            footstepInterval = Input.GetKey(KeyCode.LeftShift) ? 0.3f : 0.5f;
            controller.Move(moveDir.normalized * finalSpeed * Time.deltaTime);

            if (moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            normalizedSpeed = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;

        }

        anim.SetFloat("Speed", normalizedSpeed);

        if (holdingMovementInput)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayRandomFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        if (isStreetScene && currentCam != null)
        {
            Vector3 targetCamPos = transform.position - transform.forward * cameraDistance + Vector3.up * cameraHeight;
            currentCam.transform.position = Vector3.Lerp(currentCam.transform.position, targetCamPos, cameraFollowSpeed * Time.deltaTime);
            currentCam.transform.LookAt(transform.position + Vector3.up * 1.5f);
        }

 

    }

    void PlayRandomFootstep()
    {
        SoundID StepSound = (Random.value > 0.5f) ? SoundID.Step1Sound : SoundID.Step2Sound;
        SoundManager.instance.PlaySound(StepSound, false, Random.Range(0.3f, 0.5f));
    }


}
