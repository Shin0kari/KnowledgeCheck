using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class LoadingScreenController
{
    private ISceneLoader _sceneLoader;

    private bool isStartLoadAnimationOver = false;

    public event Action<float> OnProgressChanged;
    public event Action OnStartAnimation;
    public event Action OnEndAnimation;

    [Inject]
    private void Construct(ISceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public async UniTask AsyncChangeScene(string sceneName)
    {
        isStartLoadAnimationOver = false;

        OnStartAnimation?.Invoke();

        var loadSceneOperation = await LoadSceneAsync(sceneName);

        // Ожидаем когда с другой сцены подпишется LoadingScreenView
        while (!loadSceneOperation.isDone)
        {
            await UniTask.Yield();
        }
        OnEndAnimation?.Invoke();
    }

    private async UniTask<AsyncOperation> LoadSceneAsync(string sceneName)
    {
        var loadSceneOperation = _sceneLoader.LoadSceneAsync(sceneName);
        loadSceneOperation.allowSceneActivation = false;

        while (loadSceneOperation.progress < 0.9f || !isStartLoadAnimationOver)
        {
            OnProgressChanged?.Invoke(loadSceneOperation.progress);

            await UniTask.Yield();
        }

        loadSceneOperation.allowSceneActivation = true;

        return loadSceneOperation;
    }

    public void OnStartLoadAnimationOver()
    {
        isStartLoadAnimationOver = true;
    }
}
