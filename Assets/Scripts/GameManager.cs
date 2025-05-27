using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    // Puedes hacer el getter público para acceder a la instancia
    public static GameManager Instance
    {
        get
        {
            // Si la instancia ya existe, la devolvemos
            if (_instance != null)
            {
                return _instance;
            }

            // Si no existe, la buscamos en la escena (puede que ya esté ahí pero aún no haya llamado a Awake)
            _instance = FindObjectOfType<GameManager>();

            // Si aún no la encontramos (porque no hay una en la escena o no ha llamado a Awake),
            // creamos una nueva.
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
        // Esta es la lógica principal de un Singleton:
        // Si ya hay una instancia y NO es esta, destruye este GameObject.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Se intentó crear una segunda instancia de GameManager. Destruyendo duplicado.");
        }
        else
        {
            // Si _instance es null O si esta es la única instancia, entonces esta es la instancia oficial.
            _instance = this;
            // Aseguramos que esta instancia persista entre escenas.
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager inicializado correctamente.");
        }
    }

    // --- Tus métodos de carga de escena y salida ---

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
}