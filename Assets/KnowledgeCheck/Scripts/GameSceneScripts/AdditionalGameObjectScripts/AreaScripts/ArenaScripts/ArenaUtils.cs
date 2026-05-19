using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class ArenaUtils : MonoBehaviour, IBindingSingletonComponent
{
    private IAssetProviderGetter _assetProvider;

    private LiftDataProvider _liftDataProvider;

    [SerializeField] private float _durationBattleSecTime;
    [SerializeField] private float _spawnStopOffsetSecTime;
    [SerializeField] private float _spawnStartOffsetSecTime;
    [SerializeField] private float _onWaterValveSecTime;

    [SerializeField] private float _enemiesCountAtSameTime = 2f;

    public float MaxPosX { get; private set; }
    public float MinPosX { get; private set; }
    public float MaxPosZ { get; private set; }
    public float MinPosZ { get; private set; }
    public float PosY { get; private set; }

    public float EnemiesCount { get { return _enemiesCountAtSameTime; } }

    public ArenaTimeStruct BattleTime
    {
        get
        {
            return new ArenaTimeStruct
            {
                durationSecBattleTime = _durationBattleSecTime,
                spawnStartOffsetSecTime = _spawnStartOffsetSecTime,
                spawnStopOffsetSecTime = _spawnStopOffsetSecTime,
                onWaterValveSecTime = _onWaterValveSecTime
            };
        }
    }

    private DisposableBag _dB;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider
    )
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
            .GetIBindingSingletonComponent<LiftDataProvider>()
            .OfType<IBindingSingletonComponent, LiftDataProvider>()
            .Subscribe(liftDataProvider =>
            {
                if (liftDataProvider == null)
                    return;
                _liftDataProvider = liftDataProvider;

                UpdateArenaSize();
            })
            .AddTo(ref _dB);
    }

    private void UpdateArenaSize()
    {
        MaxPosX = _liftDataProvider.NorthArenaWallPos.x;
        MinPosX = _liftDataProvider.SouthArenaWallPos.x;
        MaxPosZ = _liftDataProvider.WestArenaWallPos.z;
        MinPosZ = _liftDataProvider.EastArenaWallPos.z;
        PosY = _liftDataProvider.LiftYPos;
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}

public struct ArenaTimeStruct
{
    public float durationSecBattleTime;
    public float spawnStartOffsetSecTime;
    public float spawnStopOffsetSecTime;
    public float onWaterValveSecTime;
}