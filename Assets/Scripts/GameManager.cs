using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public int uiLockCount = 0;
    public bool IsAnyUIOpen => uiLockCount > 0;

    [SerializeField] private PlayerMovementLocker playerLocker;

    // Diccionarios para estados persistentes
    private Dictionary<string, bool> unlockedObjects = new Dictionary<string, bool>();
    private Dictionary<string, bool> completedDialogues = new Dictionary<string, bool>();

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
            //Debug.LogWarning("Se intentó crear una segunda instancia de GameManager. Destruyendo duplicado.");
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        //Debug.Log("GameManager inicializado correctamente.");
    }

    private void Update()
    {
        /*if (Input.GetKeyUp(KeyCode.R))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }*/
        if (Input.GetKeyUp(KeyCode.N))
        {
           
            SceneManager.LoadScene("park");
        }
    } 

    #region UI Lock
    public bool TryLockUI()
    {
        if (uiLockCount > 0)
            return false;

        uiLockCount = 1;
        Debug.Log("UI bloqueada. Conteo de locks: " + uiLockCount);
        if (playerLocker != null)
        {
            playerLocker.LockMovement();
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerLocker = playerObj.GetComponent<PlayerMovementLocker>();
                if (playerLocker != null)
                {
                    playerLocker.LockMovement();
                }
                else
                {
                    Debug.LogWarning("PlayerMovementLocker no se encontró en el objeto Player.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró ningún objeto con el tag 'Player'.");
            }
        } //buscamos el script dentro del player.

        return true;
    }

    public void UnlockUI()
    {
        uiLockCount = 0;
        Debug.Log("UI desbloqueada. Conteo de locks: " + uiLockCount);
        if (playerLocker != null)
            playerLocker.UnlockMovement();
    }
    #endregion

    #region Scene Management
    public void LoadScene(string sceneName)
    {
        UnlockUI();
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        UnlockUI();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextSceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UnlockUI();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerLocker = playerObj.GetComponent<PlayerMovementLocker>();
    }
    #endregion

    #region Unlockable Objects
    public void UnlockObject(string objectID)
    {
        unlockedObjects[objectID] = true;
    }

    public bool IsObjectUnlocked(string objectID)
    {
        return unlockedObjects.ContainsKey(objectID) && unlockedObjects[objectID];
    }

    public void ResetProgress()
    {
        unlockedObjects.Clear();
        completedDialogues.Clear();
    }
    #endregion

    #region Dialogues
    public void CompleteDialogue(string dialogueID)
    {
        completedDialogues[dialogueID] = true;
    }

    public bool IsDialogueCompleted(string dialogueID)
    {
        return completedDialogues.ContainsKey(dialogueID) && completedDialogues[dialogueID];
    }
    #endregion

    #region Elock
    public bool BlockEInput { get; private set; }

    public void SetBlockEInput(bool value)
    {
        BlockEInput = value;
    }
    #endregion
}