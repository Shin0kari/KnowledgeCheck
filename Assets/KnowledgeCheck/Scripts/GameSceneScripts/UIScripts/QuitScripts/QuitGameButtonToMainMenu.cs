using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class QuitGameButtonToMainMenu : QuitGameButton
{
    private ChoicedSceneLoader _sceneLoader;

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(ChoicedSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();
    }

    protected override void QuitGame()
    {
        _sceneLoader.ChangeScene(SceneUtils.SceneNames.MainMenuScene).Forget();
    }
}