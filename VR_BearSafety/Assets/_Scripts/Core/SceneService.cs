using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneService : Singleton<SceneService>
{
    public string CurrentSceneName { get; private set; }


    private void Start()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;

        SceneManager.sceneLoaded += SetActiveScene;
    }


    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByBuildIndex(0) != SceneManager.GetActiveScene())
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }

        LoadSceneAsync(sceneName);
    }

    private async void LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }



    public void ReloadCurrentScene()
    {
        LoadScene(CurrentSceneName);
    }

    private void SetActiveScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(scene);
        CurrentSceneName = scene.name;
    }
}
