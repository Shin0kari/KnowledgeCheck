using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class LoadMenuButton : MonoBehaviour, IChangeButtonInteractable, IBindingSingletonComponent
{
    [SerializeField] private Button _button;

    private IAssetProviderGetter _assetProvider;

    private GameObject _mainPanel;
    private GameObject _savePanel;

    private DisposableBag _dB;
    private UniTaskCompletionSource _mainPanelLoadedSource = new();
    private UniTaskCompletionSource _savePanelLoadedSource = new();

    private CancellationToken _ct;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        _ct = gameObject.GetCancellationTokenOnDestroy();
        BindAllTypes();

        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {
        _mainPanelLoadedSource.TrySetCanceled();
        _mainPanelLoadedSource = null;

        _savePanelLoadedSource.TrySetCanceled();
        _savePanelLoadedSource = null;

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
            .GetIBindingSingletonComponent<SavePanelLinker>()
            .OfType<IBindingSingletonComponent, SavePanelLinker>()
            .Subscribe(savePanelLinker =>
            {
                if (savePanelLinker == null)
                    return;

                _savePanel = savePanelLinker.LinkerObject;
                _savePanelLoadedSource.TrySetResult();
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
        await _savePanelLoadedSource.Task.AttachExternalCancellation(_ct);

        if (_mainPanel != null) _mainPanel.gameObject.SetActive(false);
        if (_savePanel != null) _savePanel.gameObject.SetActive(true);
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }

    public void DisableButton()
    {
        _button.interactable = false;
    }

    public void EnableButton()
    {
        _button.interactable = true;
    }
}