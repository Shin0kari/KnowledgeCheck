using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class OptionMenuButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    private IAssetProviderGetter _assetProvider;

    private GameObject _mainPanel;
    private GameObject _optionPanel;

    private DisposableBag _dB;
    private UniTaskCompletionSource _mainPanelLoadedSource = new();
    private UniTaskCompletionSource _optionPanelLoadedSource = new();

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
        _mainPanelLoadedSource.TrySetCanceled();
        _mainPanelLoadedSource = null;

        _optionPanelLoadedSource.TrySetCanceled();
        _optionPanelLoadedSource = null;

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<MainPanelLinker>()
            .OfType<IBindingSingletonComponent, MainPanelLinker>()
            .Subscribe(mainPanelLinker =>
            {
                if (mainPanelLinker == null)
                    return;

                _mainPanel = mainPanelLinker.LinkerObject;
                _mainPanelLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<OptionPanelLinker>()
            .OfType<IBindingSingletonComponent, OptionPanelLinker>()
            .Subscribe(optionPanelLinker =>
            {
                if (optionPanelLinker == null)
                    return;

                _optionPanel = optionPanelLinker.LinkerObject;
                _optionPanelLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            AsyncOpenLoadPanel().Forget();
        });
    }

    private async UniTask AsyncOpenLoadPanel()
    {
        await _mainPanelLoadedSource.Task.AttachExternalCancellation(_ct);
        await _optionPanelLoadedSource.Task.AttachExternalCancellation(_ct);

        _mainPanel.gameObject.SetActive(false);
        _optionPanel.gameObject.SetActive(true);
    }
}