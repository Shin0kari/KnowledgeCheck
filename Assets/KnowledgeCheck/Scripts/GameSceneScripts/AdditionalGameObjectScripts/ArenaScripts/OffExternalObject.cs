using System;
using UnityEngine;
using Zenject;

public class OffExternalObject : MonoBehaviour, IDisposable
{
    private IStateController _stateController;

    [Inject]
    private void Construct(IStateController stateController)
    {
        _stateController = stateController;

        _stateController.OnOnState += OnState;
        _stateController.OnOffState += OffState;
    }

    public void Dispose()
    {
        _stateController.OnOnState -= OnState;
        _stateController.OnOffState -= OffState;
    }

    private void OnState()
    {
        gameObject.SetActive(true);
    }

    private void OffState()
    {
        gameObject.SetActive(false);
    }
}