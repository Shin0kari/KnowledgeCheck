using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : ISceneLoader
{
    public AsyncOperation LoadSceneAsync(string sceneName)
    {
        return SceneManager.LoadSceneAsync(sceneName);
    }
}