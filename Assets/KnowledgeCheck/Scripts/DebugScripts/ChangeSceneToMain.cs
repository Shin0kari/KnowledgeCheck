using UnityEngine;
using UnityEngine.SceneManagement;

using static SceneUtils;

public class ChangeSceneToMain : MonoBehaviour
{

    public void ChangeScene()
    {
        SceneNames sceneName = SceneNames.MainMenuScene;
        SceneManager.LoadSceneAsync(sceneName.ToString());
    }
}