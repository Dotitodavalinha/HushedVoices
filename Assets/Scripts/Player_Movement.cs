using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private CharacterController controller;
    private Camera currentCam;
    private Camera lastCam;
    private bool holdingMovementInput;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentCam = Camera.main;
    }

    void Update()
    {
        // Input
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        // Detectar si hay input
        holdingMovementInput = input.magnitude >= 0.1f;

        // Cambio de cámara: solo aceptar nueva cámara si no hay input
        if (!holdingMovementInput && Camera.main != currentCam)
        {
            currentCam = Camera.main;
        }

        // Si hay input, guardar la cámara actual como referencia para mover
        if (holdingMovementInput)
        {
            lastCam = currentCam;
        }

        // Usar la última cámara válida
        Camera camToUse = lastCam != null ? lastCam : currentCam;
        Vector3 camForward = camToUse.transform.forward;
        Vector3 camRight = camToUse.transform.right;

        // Ignorar inclinación vertical
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Dirección de movimiento
        Vector3 moveDir = camForward * input.z + camRight * input.x;

        if (holdingMovementInput)
        {
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
}
