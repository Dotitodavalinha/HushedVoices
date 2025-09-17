using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanItem : MonoBehaviour
{
    [SerializeField] private List<GameObject> outlinerCubes = new List<GameObject>();

    private bool InRange = false;
    private bool NoteIsOpen = false;

    private GameObject Player;
    PlayerMovementLocker playerMovementLocker;
    Animator playerAnimator;

    [SerializeField] public GameObject PressE_UI;
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] public GameObject ObjectToDestroy;

    public GameObject NoteImage;


    [SerializeField] private float destroyAfterSeconds = 1f; // configurable desde el editor


    void Start()
    {
        PressE_UI.SetActive(false);
        if (NoteImage != null)
        {
            NoteImage.SetActive(false);

        }

        Player = GameObject.FindWithTag("Player");

        playerMovementLocker = Player.GetComponent<PlayerMovementLocker>();
        playerAnimator = Player.GetComponentInChildren<Animator>();

    }

    void Update()
    {
        float distancia = Vector3.Distance(Player.transform.position, transform.position);

        if (zonaInteraccion.jugadorDentro)
        {

            InRange = true;
            if (Input.GetKeyDown(KeyCode.E))
            {
                ShowNote();
            }
        }
        else if (!zonaInteraccion.jugadorDentro)
        {

            InRange = false;
        }
        if (InRange && NoteIsOpen == false)
        {
            PressE_UI.SetActive(true);

        }
        if (!InRange)
        {
            PressE_UI.SetActive(false);
        }

        if (NoteIsOpen)
        {
            if (NoteImage != null)
            {
                NoteImage.SetActive(true);
            }

            PressE_UI.SetActive(false);
        }
        if (!NoteIsOpen)
        {
            if (NoteImage != null)
            {
                NoteImage.SetActive(false);
            }
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

        if (playerMovementLocker != null) playerMovementLocker.UnlockMovement();
        if (playerAnimator != null) playerAnimator.SetBool("IsInteracting", false);


        Destroy(ObjectToDestroy);
    }

}
