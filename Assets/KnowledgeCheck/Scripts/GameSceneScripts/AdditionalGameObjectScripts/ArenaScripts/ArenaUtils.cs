using UnityEngine;

public class ArenaUtils : MonoBehaviour
{
    [SerializeField] GameObject _northArenaWall;
    [SerializeField] GameObject _eastArenaWall;
    [SerializeField] GameObject _southArenaWall;
    [SerializeField] GameObject _westArenaWall;
    [SerializeField] GameObject _yHeightSpawnPos;

    [SerializeField] private float _durationBattleSecTime;
    [SerializeField] private float _spawnStopOffsetSecTime;
    [SerializeField] private float _spawnStartOffsetSecTime;
    [SerializeField] private float _onWaterValveSecTime;

    [SerializeField] private float _enemiesCountAtSameTime = 2f;

    private void Awake()
    {
        MaxPosX = _northArenaWall.transform.position.x;
        MinPosX = _southArenaWall.transform.position.x;
        MaxPosZ = _westArenaWall.transform.position.z;
        MinPosZ = _eastArenaWall.transform.position.z;
        PosY = _yHeightSpawnPos.transform.position.y;
    }

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
}

public struct ArenaTimeStruct
{
    public float durationSecBattleTime;
    public float spawnStartOffsetSecTime;
    public float spawnStopOffsetSecTime;
    public float onWaterValveSecTime;
}