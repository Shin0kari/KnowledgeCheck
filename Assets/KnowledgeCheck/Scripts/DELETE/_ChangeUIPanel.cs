using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChangeUIPanel : MonoBehaviour
{
    [SerializeField] private bool _isInventoryPanelVisibleOnPress;
    [SerializeField] private bool _isSaveGamePanelVisibleOnPress;
    [SerializeField] private bool _isLoadGamePanelVisibleOnPress;
    [SerializeField] private bool _isSettingsPanelVisibleOnPress;

    [SerializeField] private Button _button;

    private IAssetProviderGetter _assetProvider;

    private GameObject _inventoryPanel;
    private GameObject _saveGamePanel;
    private GameObject _loadGamePanel;
    private GameObject _settingsPanel;

    private bool _isListenerSet = false;

    private DisposableBag _dB;
    private UniTaskCompletionSource _inventoryPanelLoadedSource = new();
    private UniTaskCompletionSource _saveGamePanelLoadedSource = new();
    private UniTaskCompletionSource _loadGamePanelLoadedSource = new();
    private UniTaskCompletionSource _settingsPanelLoadedSource = new();
    private CancellationToken _ct;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        _ct = this.GetCancellationTokenOnDestroy();

        SubscribeOnUpdateObjects();
    }

    private void Start()
    {
        AsyncSetButtonListener().Forget();
    }

    private void OnDestroy()
    {
        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        _assetProvider
            .GetIBindingSingletonComponent<InventoryPanelLinker>()
            .OfType<IBindingSingletonComponent, InventoryPanelLinker>()
            .Subscribe(inventoryPanelLinker =>
            {
                if (inventoryPanelLinker == null)
                    return;

                _inventoryPanel = inventoryPanelLinker.LinkerObject;
                _inventoryPanelLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<SaveGamePanelLinker>()
            .OfType<IBindingSingletonComponent, SaveGamePanelLinker>()
            .Subscribe(saveGamePanelLinker =>
            {
                if (saveGamePanelLinker == null)
                    return;

                _saveGamePanel = saveGamePanelLinker.LinkerObject;
                _saveGamePanelLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<LoadGamePanelLinker>()
            .OfType<IBindingSingletonComponent, LoadGamePanelLinker>()
            .Subscribe(loadGamePanelLinker =>
            {
                if (loadGamePanelLinker == null)
                    return;

                _loadGamePanel = loadGamePanelLinker.LinkerObject;
                _loadGamePanelLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<SettingsPanelLinker>()
            .OfType<IBindingSingletonComponent, SettingsPanelLinker>()
            .Subscribe(settingsPanelLinker =>
            {
                if (settingsPanelLinker == null)
                    return;

                _settingsPanel = settingsPanelLinker.LinkerObject;
                _settingsPanelLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private async UniTask AsyncSetButtonListener()
    {
        if (_isListenerSet) return;
        _isListenerSet = true;

        List<UniTask> tasks = new()
        {
            _inventoryPanelLoadedSource.Task.AttachExternalCancellation(_ct),
            _saveGamePanelLoadedSource.Task.AttachExternalCancellation(_ct),
            _loadGamePanelLoadedSource.Task.AttachExternalCancellation(_ct),
            _settingsPanelLoadedSource.Task.AttachExternalCancellation(_ct),
        };

        await UniTask.WhenAll(tasks).AttachExternalCancellation(_ct);

        _button.onClick.AddListener(() =>
        {
            _inventoryPanel.SetActive(_isInventoryPanelVisibleOnPress);
            _saveGamePanel.SetActive(_isSaveGamePanelVisibleOnPress);
            _loadGamePanel.SetActive(_isLoadGamePanelVisibleOnPress);
            _settingsPanel.SetActive(_isSettingsPanelVisibleOnPress);
        });
    }
}