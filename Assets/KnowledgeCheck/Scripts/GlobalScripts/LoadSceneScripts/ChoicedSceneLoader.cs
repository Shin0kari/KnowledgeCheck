using Cysharp.Threading.Tasks;
using Zenject;
using static SceneUtils;

public class ChoicedSceneLoader
{
    private LoadingScreenController _loadingScreenController;

    [Inject]
    private void Construct(LoadingScreenController loadingScreenController)
    {
        _loadingScreenController = loadingScreenController;
    }

    public async UniTask ChangeScene(SceneNames sceneName)
    {
        await _loadingScreenController.AsyncChangeScene(sceneName.ToString());
    }
}