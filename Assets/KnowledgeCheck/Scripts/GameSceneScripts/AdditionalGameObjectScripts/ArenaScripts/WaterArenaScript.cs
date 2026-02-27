using System;
using UnityEngine;
using Zenject;

public class WaterArenaScript : MonoBehaviour, IDisposable
{
    [SerializeField] private GameObject _water;
    [SerializeField] private GameObject _maxWaterHeight;
    [SerializeField] private float _waterUpSpeed = 5f;

    private bool _isWaterUp = false;
    private ArenaController _arenaController;

    [Inject]
    private void Construct(ArenaController arenaController)
    {
        _arenaController = arenaController;

        _arenaController.SetOnWaterPipe += UpWater;
    }

    public void Dispose()
    {
        _arenaController.SetOnWaterPipe -= UpWater;
    }

    private void FixedUpdate()
    {
        if (!_isWaterUp)
            return;

        _water.transform.Translate(new(0f, _waterUpSpeed * Time.fixedDeltaTime, 0f));

        if (_water.transform.position.y >= _maxWaterHeight.transform.position.y)
            _isWaterUp = false;
    }

    private void UpWater()
    {
        _isWaterUp = true;
        _water.SetActive(true);
    }
}