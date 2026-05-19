using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static SceneUtils;

public class GameStarter : IDisposable
{
    private ChoicedSceneLoader _choicedSceneLoader;

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(ChoicedSceneLoader choicedSceneLoader)
    {
        _choicedSceneLoader = choicedSceneLoader;
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();
    }

    public void StartGame((string, SaveData) currentSave, SceneNames sceneName)
    {
        _choicedSceneLoader.ChangeScene(sceneName).Forget();
    }
}
