using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CloseMiniGame : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] private HoldMiniGameButton _miniGame;
    [SerializeField] private GameObject _backgroundMiniGame;

    private IAssetProviderGetter _assetProvider;

    private ButtonItemGenerator _buttonItemGenerator;

    private DisposableBag _dB;
    private UniTaskCompletionSource _buttonItemGeneratorLoadedSource = new();
    private CancellationToken _ct;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        _ct = gameObject.GetCancellationTokenOnDestroy();

        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {
        _buttonItemGeneratorLoadedSource.TrySetCanceled();
        _buttonItemGeneratorLoadedSource = null;

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<ButtonItemGenerator>()
            .OfType<IBindingSingletonComponent, ButtonItemGenerator>()
            .Subscribe(buttonItemGenerator =>
            {
                if (buttonItemGenerator == null)
                    return;

                _buttonItemGenerator = buttonItemGenerator;
                _buttonItemGeneratorLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            StopMiniGame();
        });
    }

    private void StopMiniGame()
    {
        AsyncEnableButton().Forget();
        _buttonItemGenerator.EnableButton();
        _backgroundMiniGame.SetActive(false);
        _miniGame.gameObject.SetActive(false);
    }

    private async UniTask AsyncEnableButton()
    {
        await _buttonItemGeneratorLoadedSource.Task.AttachExternalCancellation(_ct);
        _buttonItemGenerator.EnableButton();
    }
}