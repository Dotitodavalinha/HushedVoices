using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public int uiLockCount = 0;
    public bool IsAnyUIOpen => uiLockCount > 0;

    [SerializeField] private PlayerMovementLocker playerLocker;

    public static GameManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = FindObjectOfType<GameManager>();

            if (_instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<GameManager>();
                singletonObject.name = typeof(GameManager).ToString() + " (Singleton)";
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Se intentó crear una segunda instancia de GameManager. Destruyendo duplicado.");
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

       

        Debug.Log("GameManager inicializado correctamente.");
    }

    private void Update()
    {
        if (playerLocker != null)
        {
            return;
        }
        else
        {
            // Buscar PlayerMovementLocker en el Player
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerLocker = playerObj.GetComponent<PlayerMovementLocker>();
        }
    }

    public bool TryLockUI()
    {
        if (uiLockCount > 0)
            return false; // si ya a hay algo abierto no abrir

        uiLockCount = 1;
        if (playerLocker != null)
            playerLocker.LockMovement();
        return true;
    }

    public void UnlockUI()
    {
        uiLockCount = 0;
        if (playerLocker != null)
            playerLocker.UnlockMovement();
    }


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
    }

    public void FinalScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 4;
    }
}