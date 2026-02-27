using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class Book : MonoBehaviour, IDisposable
{
    // На случай онлайна
    List<GameObject> _targetObjects = new();
    private GameObject _newTarget;

    [SerializeField] private StarterArenaEvent _starter;
    [SerializeField] private BookAnimation _bookAnimation;

    private ArenaController _arenaController;
    private bool _isBattleStarted = false;
    private Rigidbody _rigidbody;

    [Inject]
    private void Construct(ArenaController arenaController)
    {
        _arenaController = arenaController;
        _rigidbody = GetComponent<Rigidbody>();

        _starter.PlayerOnStarter += UpdateBook;
        _starter.PlayerLeftStarter += UpdateBook;
        _arenaController.StartArenaBattle += OnBattleStarted;
    }

    public void Dispose()
    {
        if (_starter == null)
            return;

        _starter.PlayerOnStarter -= UpdateBook;
        _starter.PlayerLeftStarter -= UpdateBook;
    }

    private void UpdateBook(Player player)
    {
        if (_isBattleStarted)
            return;

        if (_targetObjects.Contains(player.gameObject))
            _targetObjects.Remove(player.gameObject);
        else
            _targetObjects.Add(player.gameObject);

        UpdateCurrentBookTarget();
    }

    private void OnBattleStarted()
    {
        _isBattleStarted = true;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _bookAnimation.OffBookAnimation();
    }

    private void UpdateCurrentBookTarget()
    {
        if (_targetObjects.Count > 0)
            _newTarget = _targetObjects[0];
        else
            _newTarget = null;

        _bookAnimation.SetNewTargetObject(_newTarget);
    }
}