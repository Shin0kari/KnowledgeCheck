using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class WinUIUtils : MonoBehaviour, IBindingSingletonComponent
{
    private IAssetProviderGetter _assetProvider;
    [SerializeField] private float _whiteWindowFadeDuration;
    [SerializeField] private float _winWindowFadeDuration;

    [SerializeField] private bool _isWinUIFadeDurFromEnemySpawnStopOffset = true;

    private DisposableBag _dB;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        BindAllTypes();
        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {
        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<ArenaUtils>()
            .OfType<IBindingSingletonComponent, ArenaUtils>()
            .Subscribe(arenaUtils =>
            {
                if (arenaUtils == null)
                    return;

                if (_isWinUIFadeDurFromEnemySpawnStopOffset)
                    _whiteWindowFadeDuration = arenaUtils.BattleTime.spawnStopOffsetSecTime;
            })
            .AddTo(ref _dB);
    }

    public float GetWhiteUIFadeDuration() => _whiteWindowFadeDuration;
    public float GetWinUIFadeDuration() => _winWindowFadeDuration;

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}