using System.Collections;
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
    private bool isPoliceStationScene;
    public Animator anim;

    [Header("Debug")]
    public bool debugInputValues = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
        currentCam = Camera.main;

        UpdateSceneFlags(SceneManager.GetActiveScene().name);

        //suscribir
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateSceneFlags(scene.name);

        // actualizar referencia de cámara
        currentCam = Camera.main;
        lastCam = currentCam;

        //si venimos por una transición con entryPoint, esperar 1 frame y resetear controller
        if (SceneTransitionManager.Instance != null && !string.IsNullOrEmpty(SceneTransitionManager.Instance.lastEntryPointID))
        {
            StartCoroutine(ResetControllerNextFrame());
        }
    }

    private void UpdateSceneFlags(string sceneName)
    {
        isRoomScene = sceneName == "Room";
        isRoomInicioScene = sceneName == "RoomInicio";
        isStreetScene = sceneName == "Street";
        isPoliceStationScene = sceneName == "StationInside";
    }

    IEnumerator ResetControllerNextFrame()
    {
        // esperar un frame para que otros scripts posicionen al player
        yield return null;
        if (controller != null)
        {
            controller.enabled = false;
            // esperar otro frame para estar seguros
            yield return null;
            controller.enabled = true;
        }

        // limpiar entryPoint para que no se vuelva a aplicar
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.lastEntryPointID = null;
    }

    void Update()
    {
        float normalizedSpeed = 0f;

        float horiz = 0f, vert = 0f;
        bool runHeld = false;

        // Preferir PersistentInput (nuevo Input System)
        if (PersistentInput.Instance != null)
        {
            horiz = PersistentInput.Instance.Horizontal;
            vert = PersistentInput.Instance.Vertical;
            runHeld = PersistentInput.Instance.Run;

            if (debugInputValues)
                Debug.Log($"[Player] PersistentInput -> H:{horiz} V:{vert} Run:{runHeld}");
        }
        else
        {
            // Fallback por seguridad (antiguo sistema)
            horiz = Input.GetAxisRaw("Horizontal");
            vert = Input.GetAxisRaw("Vertical");
            runHeld = Input.GetKey(KeyCode.LeftShift);

            if (debugInputValues)
                Debug.Log($"[Player] Fallback Input -> H:{horiz} V:{vert} Run:{runHeld}");
        }

        Vector3 input = new Vector3(horiz, 0f, vert).normalized;
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

        if (isRoomScene || isRoomInicioScene || isPoliceStationScene)
        {
            moveDir = Vector3.forward * -input.x + Vector3.right * input.z;
        }
        else
        {
            moveDir = Vector3.forward * input.z + Vector3.right * input.x;
        }

        if (holdingMovementInput)
        {
            float speedMultiplier = runHeld ? RunMagnitude : 1f;
            finalSpeed = moveSpeed * speedMultiplier;
            footstepInterval = runHeld ? 0.3f : 0.5f;
            controller.Move(moveDir.normalized * finalSpeed * Time.deltaTime);

            if (moveDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            normalizedSpeed = runHeld ? 2f : 1f;
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
