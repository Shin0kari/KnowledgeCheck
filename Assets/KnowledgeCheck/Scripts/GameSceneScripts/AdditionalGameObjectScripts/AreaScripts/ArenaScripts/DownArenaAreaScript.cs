using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class DownArenaAreaScript : MonoBehaviour
{
    private IAssetProviderGetter _assetProvider;
    private LiftDataProvider _liftDataProvider;
    private MaxLiftHeightMarkProvider _maxLiftHeightMarkProvider;

    [SerializeField] private float _downMoveSpeed;

    private ArenaController _arenaController;

    private bool _isMoveAreaStarted = false;
    private bool _isLiftDataSet = false;
    private bool _isMaxHeightMarkSet = false;

    private DisposableBag _dB;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        ArenaController arenaController
    )
    {
        _assetProvider = assetProvider;
        _arenaController = arenaController;

        SubscribeOnUpdateObjects();
        _arenaController.StartArenaBattle += StartMoveDownArea;
    }

    private void OnDestroy()
    {
        if (_arenaController != null)
            _arenaController.StartArenaBattle -= StartMoveDownArea;

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<LiftDataProvider>()
            .OfType<IBindingSingletonComponent, LiftDataProvider>()
            .Subscribe(liftDataProvider =>
            {
                if (liftDataProvider == null)
                    return;
                _liftDataProvider = liftDataProvider;
                _isLiftDataSet = true;

            })
            .AddTo(ref _dB);

        _assetProvider
            .GetIBindingSingletonComponent<MaxLiftHeightMarkProvider>()
            .OfType<IBindingSingletonComponent, MaxLiftHeightMarkProvider>()
            .Subscribe(maxLiftHeightMarkProvider =>
            {
                if (maxLiftHeightMarkProvider == null)
                    return;
                _maxLiftHeightMarkProvider = maxLiftHeightMarkProvider;
                _isMaxHeightMarkSet = true;
            })
            .AddTo(ref _dB);
    }

    private void StartMoveDownArea()
    {
        _isMoveAreaStarted = true;

    }

    private void Update()
    {
        if (!_isMoveAreaStarted)
            return;

        if (!_isLiftDataSet || !_isMaxHeightMarkSet)
            return;

        if (_liftDataProvider.LiftYPos >= _maxLiftHeightMarkProvider.MaxYLiftHeight)
            _isMoveAreaStarted = false;

        transform.Translate(new(0f, -_downMoveSpeed * Time.deltaTime, 0f));
    }
}