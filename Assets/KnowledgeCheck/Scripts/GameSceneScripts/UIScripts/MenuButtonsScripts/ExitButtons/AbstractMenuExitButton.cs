using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public abstract class AbstractMenuExitButton : MonoBehaviour
{
    [SerializeField] protected Button _button;

    private IAssetProviderGetter _assetProvider;

    protected GameObject _quitWarningPanel;
    protected QuitButtonPressedChecker _quitButtonPressedChecker;
    protected GameObject _backgroundDeniedPanel;

    private DisposableBag _dB;
    protected UniTaskCompletionSource _quitWarningPanelLinkerLoadedSource = new();
    protected UniTaskCompletionSource _backgroundDeniedPanelLinkerLoadedSource = new();
    protected CancellationToken _ct = new();

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        _ct = this.GetCancellationTokenOnDestroy();

        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {
        _quitWarningPanelLinkerLoadedSource.TrySetCanceled();
        _quitWarningPanelLinkerLoadedSource = null;

        _backgroundDeniedPanelLinkerLoadedSource.TrySetCanceled();
        _backgroundDeniedPanelLinkerLoadedSource = null;

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        _assetProvider
            .GetIBindingSingletonComponent<QuitWarningPanelLinker>()
            .OfType<IBindingSingletonComponent, QuitWarningPanelLinker>()
            .Subscribe(quitWarningPanelLinker =>
            {
                if (quitWarningPanelLinker == null)
                    return;

                _quitWarningPanel = quitWarningPanelLinker.LinkerObject;
                if (!_quitWarningPanel.TryGetComponent(out QuitButtonPressedChecker quitButtonPressedChecker))
                {
                    ErrorMessageGenerator.GenerateSimpleError(this, "QuitWarningPanel haven`t QuitButtonPressedChecker");
                }
                _quitButtonPressedChecker = quitButtonPressedChecker;

                _quitWarningPanelLinkerLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<BackgroundDeniedPanelLinker>()
            .OfType<IBindingSingletonComponent, BackgroundDeniedPanelLinker>()
            .Subscribe(backgroundDeniedPanelLinker =>
            {
                if (backgroundDeniedPanelLinker == null)
                    return;

                _backgroundDeniedPanel = backgroundDeniedPanelLinker.LinkerObject;

                _backgroundDeniedPanelLinkerLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private void Start()
    {
        AsyncSetLisnener().Forget();
    }

    protected virtual async UniTask AsyncSetLisnener()
    {
        List<UniTask> tasks = new()
        {
            _quitWarningPanelLinkerLoadedSource.Task.AttachExternalCancellation(_ct),
            _backgroundDeniedPanelLinkerLoadedSource.Task.AttachExternalCancellation(_ct),
        };

        await UniTask.WhenAll(tasks).AttachExternalCancellation(_ct);
    }
}