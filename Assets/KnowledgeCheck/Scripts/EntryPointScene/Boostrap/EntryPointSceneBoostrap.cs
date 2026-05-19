using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class EntryPointSceneBoostrap
{
    private ChangeScene _sceneChanger;

    [Inject]
    private void Construct(ChangeScene sceneChanger)
    {
        _sceneChanger = sceneChanger;

        _sceneChanger.LoadMenuScene().Forget();
    }
}