using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public abstract class AbstractMenuButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    protected IAssetProviderGetter _assetProvider;
    private SecondPanelUILinkerManager _secondPanelUILinkerManager;

    protected GameObject _linkedPanel;
    private bool _isListenerSet = false;

    protected DisposableBag _dB;
    protected UniTaskCompletionSource _linkedPanelLoadedSource = new();
    private CancellationToken _ct;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        SecondPanelUILinkerManager secondPanelUILinkerManager
    )
    {
        _assetProvider = assetProvider;
        _secondPanelUILinkerManager = secondPanelUILinkerManager;

        _ct = this.GetCancellationTokenOnDestroy();

        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {
        _linkedPanelLoadedSource.TrySetCanceled();
        _linkedPanelLoadedSource = null;

        _dB.Dispose();
    }

    protected abstract void SubscribeOnUpdateObjects();

    private void Start()
    {
        AsyncSetButtonListener().Forget();
    }

    private async UniTask AsyncSetButtonListener()
    {
        if (_isListenerSet) return;
        _isListenerSet = true;

        await _linkedPanelLoadedSource.Task.AttachExternalCancellation(_ct);

        _button.onClick.AddListener(() =>
        {
            _secondPanelUILinkerManager.OffAllPanels();
            _linkedPanel.SetActive(true);
        });
    }
}