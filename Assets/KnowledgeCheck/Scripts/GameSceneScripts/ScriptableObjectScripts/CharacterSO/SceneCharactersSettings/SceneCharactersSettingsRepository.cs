using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class SceneCharactersSettingsRepository : IDisposable
{
    private SceneCharactersSettingsProvider _sceneCharactersSettingsProvider;

    private ReactiveProperty<ScriptableObject> _reactiveSceneCharactersSettingsSO;
    private SceneCharactersSettingsSO _sceneCharactersSettingsSO;

    private DisposableBag _dB;
    private UniTaskCompletionSource _sceneCharactersSettingsSOLoadedSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(SceneCharactersSettingsProvider SceneCharactersSettingsProvider)
    {
        _sceneCharactersSettingsProvider = SceneCharactersSettingsProvider;

        AsyncLoadResource().Forget();
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();
        _ct = null;

        _dB.Dispose();
    }

    private async UniTask AsyncLoadResource()
    {
        _reactiveSceneCharactersSettingsSO = await _sceneCharactersSettingsProvider.TryGetDataSO(_ct.Token);

        _reactiveSceneCharactersSettingsSO?
            .Subscribe(SceneCharactersSettingsSO =>
            {
                if (SceneCharactersSettingsSO == null)
                    return;

                SetSO(SceneCharactersSettingsSO).Forget();
            })
            .AddTo(ref _dB);
    }

    private async UniTask SetSO(ScriptableObject SceneCharactersSettingsSO)
    {
        if (SceneCharactersSettingsSO is not SceneCharactersSettingsSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        _sceneCharactersSettingsSO = so;

        _sceneCharactersSettingsSO.SetDefaultState();
        await _sceneCharactersSettingsSO.LoadAllConfigs(_ct.Token);
        _sceneCharactersSettingsSOLoadedSource.TrySetResult();
    }

    public async UniTask<SceneCharactersSettingsSO> AsyncGetSceneCharactersSettingsSO(CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );
        await _sceneCharactersSettingsSOLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);

        return _sceneCharactersSettingsSO;
    }
}