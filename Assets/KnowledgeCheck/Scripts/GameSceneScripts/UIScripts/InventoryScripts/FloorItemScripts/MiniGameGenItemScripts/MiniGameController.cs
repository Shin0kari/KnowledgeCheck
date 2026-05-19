using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class MiniGameController : MonoBehaviour
{
    [SerializeField] private HoldMiniGameButton _miniGame;
    [SerializeField] private CongratulationPanel _congratulationsPanel;

    [SerializeField] private GameObject _backgroundMiniGame;
    [SerializeField] private GameObject _rotatedMiniGameObject;

    private IAssetProviderGetter _assetProvider;

    private ButtonRechargeAnimation _buttonRechargeAnimation;
    private ButtonItemGenerator _buttonItemGenerator;

    private FloorItemSpawner _floorItemSpawner;
    private Vector3 _startMiniGamePos;

    private DisposableBag _dB;
    private UniTaskCompletionSource _buttonRechargeAnimationLoadedSource = new();
    private UniTaskCompletionSource _buttonItemGeneratorLoadedSource = new();
    private CancellationToken _ct;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        FloorItemSpawner floorItemSpawner
    )
    {
        _assetProvider = assetProvider;
        _floorItemSpawner = floorItemSpawner;

        _ct = gameObject.GetCancellationTokenOnDestroy();
        _startMiniGamePos = _rotatedMiniGameObject.transform.position;

        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {
        if (_buttonRechargeAnimation != null)
        {
            _buttonItemGenerator.IsUsed -= StartMiniGame;
            _buttonRechargeAnimation.OnFullCharge -= SetDefaultButtonState;
        }
        if (_miniGame != null)
            _miniGame.OnCompleteMiniGame -= StartCongratulatons;
        if (_congratulationsPanel != null)
        {
            _congratulationsPanel.OnShowCompleteText -= SetRechargeButtonState;
            _congratulationsPanel.OnShowCompleteText -= SpawnItem;
        }

        _buttonRechargeAnimationLoadedSource.TrySetCanceled();
        _buttonRechargeAnimationLoadedSource = null;

        _buttonItemGeneratorLoadedSource.TrySetCanceled();
        _buttonItemGeneratorLoadedSource = null;

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<ButtonRechargeAnimation>()
            .OfType<IBindingSingletonComponent, ButtonRechargeAnimation>()
            .Subscribe(buttonRechargeAnimation =>
            {
                if (buttonRechargeAnimation == null)
                    return;

                if (_buttonRechargeAnimation != null)
                    _buttonRechargeAnimation.OnFullCharge -= SetDefaultButtonState;

                _buttonRechargeAnimation = buttonRechargeAnimation;
                _buttonRechargeAnimation.OnFullCharge += SetDefaultButtonState;

                _buttonRechargeAnimationLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<ButtonItemGenerator>()
            .OfType<IBindingSingletonComponent, ButtonItemGenerator>()
            .Subscribe(buttonItemGenerator =>
            {
                if (buttonItemGenerator == null)
                    return;

                if (_buttonItemGenerator != null)
                    _buttonItemGenerator.IsUsed -= StartMiniGame;

                _buttonItemGenerator = buttonItemGenerator;
                _buttonItemGenerator.IsUsed += StartMiniGame;
                _buttonItemGeneratorLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private void Start()
    {
        _miniGame.OnCompleteMiniGame += StartCongratulatons;
        _congratulationsPanel.OnShowCompleteText += SetRechargeButtonState;
        _congratulationsPanel.OnShowCompleteText += SpawnItem;
    }

    private void StartMiniGame()
    {
        SetMiniGameDefaultSetting();

        AsyncDisableButton().Forget();
        _backgroundMiniGame.SetActive(true);
        _miniGame.gameObject.SetActive(true);
    }

    private async UniTask AsyncDisableButton()
    {
        await _buttonItemGeneratorLoadedSource.Task.AttachExternalCancellation(_ct);
        _buttonItemGenerator.DisableButton();
    }

    private void SetMiniGameDefaultSetting()
    {
        _rotatedMiniGameObject.transform.position = _startMiniGamePos;
        _rotatedMiniGameObject.transform.rotation = Quaternion.identity;
    }

    private void StartCongratulatons()
    {
        _miniGame.gameObject.SetActive(false);
        _backgroundMiniGame.SetActive(false);
        _congratulationsPanel.gameObject.SetActive(true);
    }

    private void SetRechargeButtonState()
    {
        _congratulationsPanel.gameObject.SetActive(false);
        AsyncStartRechargeAnimation().Forget();
    }

    private async UniTask AsyncStartRechargeAnimation()
    {
        await _buttonRechargeAnimationLoadedSource.Task.AttachExternalCancellation(_ct);
        _buttonRechargeAnimation.StartRecharge();
    }

    private void SpawnItem()
    {
        _floorItemSpawner.SpawnItem();
    }

    private void SetDefaultButtonState()
    {
        AsyncEnableButton().Forget();
    }

    private async UniTask AsyncEnableButton()
    {
        await _buttonItemGeneratorLoadedSource.Task.AttachExternalCancellation(_ct);
        _buttonItemGenerator.EnableButton();
    }
}