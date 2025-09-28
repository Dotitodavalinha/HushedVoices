using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanItem : MonoBehaviour
{
    [SerializeField] private List<GameObject> outlinerCubes = new List<GameObject>();

    private bool InRange = false;
    private bool NoteIsOpen = false;
    [SerializeField] public bool LockPlayer = true;

    private GameObject Player;
    private Transform feetPoint;
    PlayerMovementLocker playerMovementLocker;
    Animator playerAnimator;

    [SerializeField] public GameObject PressE_UI;
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] public GameObject ObjectToDestroy;
    public GameObject NoteBen;
    public GameObject BensReport;
    public GameObject NoteImage;

    [SerializeField] private GameObject particlePrefab;


    [SerializeField] private float destroyAfterSeconds = 1f;
    public enum CleanReward
    {
        None,
        NoteBen,
        BensReport
    }

    [SerializeField] private CleanReward reward = CleanReward.None;


    void Start()
    {
        PressE_UI.SetActive(false);
        if (NoteImage != null)
        {
            NoteImage.SetActive(false);
        }

        Player = GameObject.FindWithTag("Player");

        feetPoint = Player.transform.Find("FeetPoint");

        if (feetPoint == null)
        {
            Debug.LogWarning("No FeetPoint");
        }
        if (LockPlayer == true)
        {
            playerMovementLocker = Player.GetComponent<PlayerMovementLocker>();
        }

        playerAnimator = Player.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (zonaInteraccion.jugadorDentro)
        {
            InRange = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                ShowNote();
            }
        }
        else
        {
            InRange = false;
        }

        if (InRange && NoteIsOpen == false)
        {
            PressE_UI.SetActive(true);
        }
        else if (!InRange)
        {
            PressE_UI.SetActive(false);
        }

        if (NoteIsOpen)
        {
            if (NoteImage != null) NoteImage.SetActive(true);
            PressE_UI.SetActive(false);
        }
        else
        {
            if (NoteImage != null) NoteImage.SetActive(false);
        }
    }

    void ShowNote()
    {
        NoteIsOpen = !NoteIsOpen;

        if (outlinerCubes != null && outlinerCubes.Count > 0)
        {
            foreach (var cube in outlinerCubes)
            {
                if (cube != null)
                    cube.SetActive(false);
            }
        }
        else
        {
            Transform singleOutliner = transform.Find("OutlinerCube");
            if (singleOutliner != null)
                singleOutliner.gameObject.SetActive(false);
        }

        if (NoteIsOpen)
        {
            if (NoteImage != null)
            {
                if (!GameManager.Instance.TryLockUI())
                {
                    NoteIsOpen = false; // cancela apertura
                    return;
                }
            }

            if (playerMovementLocker != null) playerMovementLocker.LockMovement();
            if (playerAnimator != null) playerAnimator.SetBool("IsInteracting", true);

            if (particlePrefab != null)
            {
                GameObject particles = Instantiate(particlePrefab, feetPoint.transform.position, Quaternion.identity);
                Destroy(particles, 1f);
            }


            StartCoroutine(DestroyAfterDelay());
        }
        else
        {
            GameManager.Instance.UnlockUI();
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyAfterSeconds);

        if (CleanManager.Instance != null)
        {
            CleanManager.Instance.RegisterCleanedItem();
        }

        switch (reward)
        {
            case CleanReward.NoteBen:
                if (NoteBen != null) NoteBen.SetActive(true);
                break;

            case CleanReward.BensReport:
                if (BensReport != null) BensReport.SetActive(true);
                break;

            case CleanReward.None:
            default:
                break;
        }

        if (playerMovementLocker != null) playerMovementLocker.UnlockMovement();
        if (playerAnimator != null) playerAnimator.SetBool("IsInteracting", false);

        Destroy(ObjectToDestroy);
    }
}
