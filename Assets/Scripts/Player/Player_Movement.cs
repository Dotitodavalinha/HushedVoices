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

    private CharacterController controller;
    private Camera currentCam;
    private Camera lastCam;
    private bool holdingMovementInput;
    private bool isStreetScene;
    public Animator anim;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
        currentCam = Camera.main;
        isStreetScene = SceneManager.GetActiveScene().name == "Street";
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        holdingMovementInput = input.magnitude >= 0.1f;

        anim.SetFloat("Speed", input.magnitude);

        if (!isStreetScene && !holdingMovementInput && Camera.main != currentCam)
        {
            currentCam = Camera.main;
        }

        if (holdingMovementInput)
        {
            lastCam = currentCam;
        }

        Vector3 moveDir = Vector3.zero;

        if (isStreetScene)
        {
            moveDir = transform.forward * input.z + transform.right * input.x;
        }
        else
        {
            Camera camToUse = lastCam != null ? lastCam : currentCam;
            Vector3 camForward = camToUse.transform.forward;
            Vector3 camRight = camToUse.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDir = camForward * input.z + camRight * input.x;
        }

        if (holdingMovementInput)
        {
            float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? RunMagnitude : 1f;
            controller.Move(moveDir.normalized * moveSpeed * speedMultiplier * Time.deltaTime);

            if (moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (isStreetScene && currentCam != null)
        {
            Vector3 targetCamPos = transform.position - transform.forward * cameraDistance + Vector3.up * cameraHeight;
            currentCam.transform.position = Vector3.Lerp(currentCam.transform.position, targetCamPos, cameraFollowSpeed * Time.deltaTime);
            currentCam.transform.LookAt(transform.position + Vector3.up * 1.5f);
        }
    }
}
