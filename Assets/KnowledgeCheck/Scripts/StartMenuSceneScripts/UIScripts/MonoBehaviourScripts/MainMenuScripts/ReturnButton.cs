using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ReturnButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    // В префабе ReturnButton, currentPanel элемент должен быть пустым
    // Этот элемент заполняется уже на нужной UI панели
    [SerializeField] private GameObject _currentPanel;

    private IAssetProviderGetter _assetProvider;

    private GameObject _mainPanel;

    private DisposableBag _dB;
    private UniTaskCompletionSource _mainPanelLoadedSource = new();
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

        if (_currentPanel != null) _currentPanel.gameObject.SetActive(false);
        if (_mainPanel != null) _mainPanel.gameObject.SetActive(true);
    }
}