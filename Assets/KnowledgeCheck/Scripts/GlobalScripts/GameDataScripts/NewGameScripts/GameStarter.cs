using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using static SceneUtils;

public class GameStarter
{
    private LoadingScreenController _loadingScreenController;

    [Inject]
    private void Construct(LoadingScreenController loadingScreenController)
    {
        _loadingScreenController = loadingScreenController;
    }

    public async UniTask StartGame((string, SaveData) currentSave, SceneNames sceneName)
    {
        // Возможно удалить currentSave из получаемых данных, тк игровые данные получаются из глобального zenject installer`а
        await _loadingScreenController.AsyncChangeScene(sceneName.ToString());
    }
}
