using System.Collections;
using UnityEngine;

public class CameraDialogueZoom : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private DialogueManager dialogueScript;

    private Camera activeCamera;
    private Transform originalPos;
    private Coroutine zoomCoroutine;

    private bool hasZoomed = false;
    private bool originalPosSaved = false;

    private void Awake()
    {
        if (cameraManager == null)
            cameraManager = GetComponent<CameraManager>();
    }

    private void Start()
    {
        if (dialogueScript != null)
        {
          //  dialogueScript.OnDialogueStart += HandleDialogueStart;
          //  dialogueScript.OnDialogueEnd += HandleDialogueEnd;
        }
    }


    private void HandleDialogueStart()
    {
        if (!hasZoomed && cameraManager.GetCurrentCameraBehaviour() != null)
        {
           // ZoomToDialogue(dialogueScript.playerTransform, dialogueScript.npcTransform, 1f);
            hasZoomed = true;
        }
    }

    private void HandleDialogueEnd()
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        if (hasZoomed)
        {
            zoomCoroutine = StartCoroutine(ReturnToOriginalCoroutine(1f));
            hasZoomed = false;
        }
    }

    public void ZoomToDialogue(Transform player, Transform npc, float duration = 1f)
    {
        if (!cameraManager.IsInitialized()) return;
        Debug.Log("Zoom start");
        if (originalPos != null)
        {
            Destroy(originalPos.gameObject);
            originalPos = null;
            originalPosSaved = false;
        }

        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(ZoomCoroutine(player, npc, duration));
    }

    private IEnumerator ZoomCoroutine(Transform player, Transform npc, float duration)
    {
        MonoBehaviour camBehaviour = cameraManager.GetCurrentCameraBehaviour();
        activeCamera = camBehaviour.GetComponent<Camera>();
        if (activeCamera == null) yield break;

        Transform camTransform = activeCamera.transform;

        if (!originalPosSaved)
        {
            originalPos = new GameObject("CamOriginalPos").transform;
            originalPos.position = camTransform.position;
            originalPos.rotation = camTransform.rotation;
            originalPosSaved = true;
        }

        Vector3 midPoint = (player.position + npc.position) / 2f;
        Vector3 direction = (midPoint - camTransform.position).normalized;
        Vector3 targetPosition = midPoint - direction * 2f + Vector3.up * 1f;
        Quaternion targetRotation = Quaternion.LookRotation(midPoint - targetPosition);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            camTransform.position = Vector3.Lerp(originalPos.position, targetPosition, elapsed / duration);
            camTransform.rotation = Quaternion.Slerp(originalPos.rotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.position = targetPosition;
        camTransform.rotation = targetRotation;
    }

    private IEnumerator ReturnToOriginalCoroutine(float duration)
    {
        if (activeCamera == null || originalPos == null) yield break;

        Transform camTransform = activeCamera.transform;

        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;

        Vector3 endPos = originalPos.position;
        Quaternion endRot = originalPos.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            camTransform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            camTransform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.position = endPos;
        camTransform.rotation = endRot;

        Destroy(originalPos.gameObject);
        originalPosSaved = false;
    }
}
