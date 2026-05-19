using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class QuitGameButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    // private ISceneLoader _sceneLoader;

    // [Inject]
    // private void Construct(ISceneLoader sceneLoader)
    // {
    //     _sceneLoader = sceneLoader;
    // }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            QuitGame();
        });
    }

    protected virtual void QuitGame()
    {
        // _sceneLoader.AsyncUnloadScene();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
