using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

   
    public static GameManager Instance
    {
        get
        {
           
            if (_instance != null)
            {
                return _instance;
            }

           
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
        }
        else
        {
           
            _instance = this;
            
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager inicializado correctamente.");
        }
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