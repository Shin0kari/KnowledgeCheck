using UnityEngine;
using Zenject;

public class WinUIUtils : MonoBehaviour
{
    [SerializeField] private float _whiteWindowFadeDuration;
    [SerializeField] private float _winWindowFadeDuration;

    [SerializeField] private bool _isWinUIFadeDurFromEnemySpawnStopOffset = true;

    [Inject]
    private void Construct(ArenaUtils arenaUtils)
    {
        if (_isWinUIFadeDurFromEnemySpawnStopOffset)
            _whiteWindowFadeDuration = arenaUtils.BattleTime.spawnStopOffsetSecTime;
    }

    public float GetWhiteUIFadeDuration() => _whiteWindowFadeDuration;
    public float GetWinUIFadeDuration() => _winWindowFadeDuration;
}