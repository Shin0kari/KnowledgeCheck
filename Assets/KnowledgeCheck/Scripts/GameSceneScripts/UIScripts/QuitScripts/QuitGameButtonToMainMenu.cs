using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class QuitGameButtonToMainMenu : QuitGameButton
{
    private ChoicedSceneLoader _sceneLoader;

    [Inject]
    private void Construct(ChoicedSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    protected override void QuitGame()
    {
        UniTask asyncSceneChangeOperation = _sceneLoader.ChangeScene(SceneUtils.SceneNames.MainMenuScene);
        asyncSceneChangeOperation.Forget();
    }
}