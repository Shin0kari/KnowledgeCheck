using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static SceneUtils;

public class GameStarter
{
    private ChoicedSceneLoader _choicedSceneLoader;

    [Inject]
    private void Construct(ChoicedSceneLoader choicedSceneLoader)
    {
        _choicedSceneLoader = choicedSceneLoader;
    }

    public void StartGame((string, SaveData) currentSave, SceneNames sceneName)
    {
        UniTask asyncSceneChangeOperation = _choicedSceneLoader.ChangeScene(sceneName);
        asyncSceneChangeOperation.Forget();
    }
}
