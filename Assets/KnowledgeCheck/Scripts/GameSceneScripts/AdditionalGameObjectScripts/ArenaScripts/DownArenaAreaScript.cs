using System;
using UnityEngine;
using Zenject;

public class DownArenaAreaScript : MonoBehaviour, IDisposable
{
    [SerializeField] private GameObject _maxHeightLiftMark;
    [SerializeField] private GameObject _lift;
    [SerializeField] private float _downMoveSpeed;

    private ArenaController _arenaController;

    private bool _isMoveAreaStarted = false;

    [Inject]
    private void Construct(ArenaController arenaController)
    {
        _arenaController = arenaController;

        _arenaController.StartArenaBattle += StartMoveDownArea;
    }

    public void Dispose()
    {
        _arenaController.StartArenaBattle -= StartMoveDownArea;
    }

    private void StartMoveDownArea()
    {
        _isMoveAreaStarted = true;
    }

    private void Update()
    {
        if (!_isMoveAreaStarted)
            return;

        if (_lift.transform.position.y >= _maxHeightLiftMark.transform.position.y)
            _isMoveAreaStarted = false;

        transform.Translate(new(0f, -_downMoveSpeed * Time.deltaTime, 0f));
    }
}