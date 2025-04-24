using System.Collections;
using UnityEngine;

public class CameraDialogueZoom : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    private Camera activeCamera;
    private Transform originalPos;
    private Coroutine zoomCoroutine;

    private bool hasZoomed = false;
    private bool originalPosSaved = false;

    [SerializeField] private TextDefault dialogueScript; 

    private void Awake()
    {
        cameraManager = GetComponent<CameraManager>();
    }

    private void Update()
    {
        if (!cameraManager.IsInitialized())
            return;

        if (dialogueScript == null || dialogueScript.playerTransform == null || dialogueScript.npcTransform == null)
            return;

        if (dialogueScript.IsInteracting && !hasZoomed && cameraManager.GetCurrentCameraBehaviour() != null)
        {
            ZoomToDialogue(dialogueScript.playerTransform, dialogueScript.npcTransform, 1f, 3f);
            hasZoomed = true;
        }

        else if (!dialogueScript.IsInteracting && hasZoomed)
        {
            hasZoomed = false;
        }
    }


    public void ZoomToDialogue(Transform player, Transform npc, float duration = 1f, float holdTime = 3f)
    {
        if (!cameraManager.IsInitialized()) return;

        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(ZoomCoroutine(player, npc, duration, holdTime));
    }

    private IEnumerator ZoomCoroutine(Transform player, Transform npc, float duration, float holdTime)
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

        yield return new WaitForSeconds(holdTime);

        elapsed = 0f;
        while (elapsed < duration)
        {
            camTransform.position = Vector3.Lerp(camTransform.position, originalPos.position, elapsed / duration);
            camTransform.rotation = Quaternion.Slerp(camTransform.rotation, originalPos.rotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.position = originalPos.position;
        camTransform.rotation = originalPos.rotation;
        Destroy(originalPos.gameObject);
    }

}
