using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class OnWaterPipe : MonoBehaviour, IDisposable
{
    [SerializeField] GameObject _waterColumn;
    private ArenaController _arenaController;

    [Inject]
    private void Construct(ArenaController arenaController)
    {
        _arenaController = arenaController;

        _arenaController.SetOnWaterPipe += OnWaterValve;
    }

    private void OnWaterValve()
    {
        _waterColumn.SetActive(true);
        _waterColumn.transform.DOScaleY(0.75f, 0.25f);
    }

    public void Dispose()
    {
        _arenaController.SetOnWaterPipe -= OnWaterValve;
    }
}