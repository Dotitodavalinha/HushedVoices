using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CorchoInteract : MonoBehaviour
{
    [SerializeField] public NOTEInteractionZone zonaInteraccion;
    [SerializeField] private ClueBoardManager corchoManager;
    [SerializeField] private FolioAnimation folio;

    [SerializeField] public GameObject PressE;
    [SerializeField] private bool UI_Activa = false;

    [Header("Sistema de Cinemáticas")]
    [SerializeField] private VideoPlayer reproductorVideo;
    [SerializeField] private GameObject pantallaRawImage;
    [SerializeField] private VideoClip videoEntrada;
    [SerializeField] private VideoClip videoSalida;

    private bool yaSeReprodujoEntrada = false;
    private bool yaSeReprodujoSalida = false;

    private bool viendoVideoEntrada = false;
    private bool viendoVideoSalida = false;

    private PlayerMovementLocker movementLocker;

    private void Start()
    {
        PressE.SetActive(false);
        if (pantallaRawImage != null) pantallaRawImage.SetActive(false);

        movementLocker = FindAnyObjectByType<PlayerMovementLocker>();

        if (reproductorVideo != null)
        {
            reproductorVideo.loopPointReached += AlTerminarVideo;

            AudioSource audioVideo = reproductorVideo.GetComponent<AudioSource>();
            if (audioVideo != null)
            {
                audioVideo.ignoreListenerPause = true;
            }
        }
    }

    private void OnDestroy()
    {
        if (reproductorVideo != null)
        {
            reproductorVideo.loopPointReached -= AlTerminarVideo;
        }
    }

    public void SetUIState(bool active)
    {
        UI_Activa = active;
    }

    private void Update()
    {
        if (viendoVideoEntrada || viendoVideoSalida)
        {
            if (Input.GetKeyDown(KeyCode.E)) SaltarVideo();
            return;
        }

        if (UI_Activa)
        {
            PressE.SetActive(false);

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!yaSeReprodujoSalida && BrokenClueCleaner.allCleaners.Count > 0)
                {
                    //Debug.Log("Limpiar corcho primero");
                    return;
                }

                if (!yaSeReprodujoSalida && reproductorVideo != null && videoSalida != null)
                {
                    IniciarVideoSalida();
                }
                else
                {
                    CerrarCorchoNormal();
                }
            }
            return;
        }

        if (zonaInteraccion.jugadorDentro)
        {
            PressE.SetActive(true);

            if (GameManager.Instance.BlockEInput) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                Interactuar();
            }
        }
        else
        {
            PressE.SetActive(false);
        }
    }

    private void Interactuar()
    {
        if (!GameManager.Instance.TryLockUI()) return;

        if (!yaSeReprodujoEntrada && reproductorVideo != null && videoEntrada != null)
        {
            IniciarVideoEntrada();
        }
        else
        {
            AbrirCorchoNormal();
        }
    }

    private void IniciarVideoEntrada()
    {
        yaSeReprodujoEntrada = true;
        viendoVideoEntrada = true;

        Cursor.visible = false;
        if (movementLocker != null) movementLocker.LockMovement();

        AudioListener.pause = true;
        reproductorVideo.clip = videoEntrada;
        pantallaRawImage.SetActive(true);
        reproductorVideo.Play();
    }

    private void IniciarVideoSalida()
    {
        yaSeReprodujoSalida = true;
        viendoVideoSalida = true;

        corchoManager.CloseBoard();

        Cursor.visible = false;
        if (movementLocker != null) movementLocker.LockMovement();

        AudioListener.pause = true;
        reproductorVideo.clip = videoSalida;
        pantallaRawImage.SetActive(true);
        reproductorVideo.Play();
    }

    private void SaltarVideo()
    {
        reproductorVideo.Stop();
        AlTerminarVideo(reproductorVideo);
    }

    private void AlTerminarVideo(VideoPlayer vp)
    {
        AudioListener.pause = false;

        if (viendoVideoEntrada)
        {
            viendoVideoEntrada = false;
            pantallaRawImage.SetActive(false);
            AbrirCorchoNormal();
        }
        else if (viendoVideoSalida)
        {
            viendoVideoSalida = false;
            pantallaRawImage.SetActive(false);
            CerrarCorchoNormal();
        }
    }

    private void AbrirCorchoNormal()
    {
        UI_Activa = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        corchoManager.OpenBoard();
        folio.OnUIReopened();
        FindObjectOfType<ExitUnlocker>()?.MarcarCorchoUsado();
    }

    private void CerrarCorchoNormal()
    {
        UI_Activa = false;
        viendoVideoSalida = false;

        GameManager.Instance.UnlockUI();
        if (movementLocker != null) movementLocker.UnlockMovement();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        corchoManager.CloseBoard();
    }

    public void ForceUIState(bool active)
    {
        UI_Activa = active;
    }
}