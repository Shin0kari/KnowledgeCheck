using System;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterEventObserver))]
[RequireComponent(typeof(Enemy))]
public class EnemyNavigation : MonoBehaviour, IGetMovementSpeed, IGetRotationSpeed, IGetStopDistance, IGetSqrDestinationDistance, ISwitchAgentActivityState, IDisposable
{
    private NavMeshAgent _agent;

    private SignalBus _signalBus;
    private Player _enemyTarget;

    private PlayerEventObserver _enemyTargetEventObserver;

    private CharacterEventObserver _characterEventObserver;

    private float _movementSpeed;
    private float _rotationSpeed;
    private float _stopDistance;

    [Inject]
    private void Consturct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _characterEventObserver = GetComponent<CharacterEventObserver>();

        _agent = GetComponent<NavMeshAgent>();
        _movementSpeed = _agent.speed;
        _rotationSpeed = _agent.angularSpeed;
        _stopDistance = _agent.stoppingDistance;

        _characterEventObserver.OnLandState += StartAgent;
        _characterEventObserver.OnFallState += StopAgent;
        _characterEventObserver.OnDeath += StopAgent;
    }

    public void Dispose()
    {

        if (_characterEventObserver != null)
        {
            _characterEventObserver.OnLandState -= StartAgent;
            _characterEventObserver.OnFallState -= StopAgent;
            _characterEventObserver.OnDeath -= StopAgent;
        }

        _signalBus.TryUnsubscribe<PlayerSpawnedSignal>(SetSignalEnemyTarget);

        StopAgent();
    }

    private void OnEnable()
    {
        SetEnemyTarget();
    }

    private void OnDisable()
    {
        ClearEnemySubscribes();
    }

    private void StartAgent()
    {
        Debug.Log("StartAgent");
        _agent.enabled = true;
    }

    private void StopAgent()
    {
        _agent.enabled = false;
    }

    private void SetEnemyTarget()
    {
        _enemyTarget = GetComponent<Enemy>().GetEnemyTarget();
        if (_enemyTarget != null)
            SetEnemyTargetEvents();
        else
            _signalBus.Subscribe<PlayerSpawnedSignal>(SetSignalEnemyTarget);
    }

    private void SetSignalEnemyTarget(PlayerSpawnedSignal args)
    {
        _enemyTarget = args.Player;
        SetEnemyTargetEvents();
    }

    private void SetEnemyTargetEvents()
    {
        _enemyTargetEventObserver = _enemyTarget.gameObject.GetComponent<PlayerEventObserver>();
        _enemyTargetEventObserver.OnDeath += StopNavigation;
    }

    private void StopNavigation()
    {
        ClearEnemySubscribes();
        StopAgent();
    }

    private void ClearEnemySubscribes()
    {
        if (_enemyTargetEventObserver != null)
            _enemyTargetEventObserver.OnDeath -= StopNavigation;
        _signalBus.TryUnsubscribe<PlayerSpawnedSignal>(SetSignalEnemyTarget);
    }

    private void Update()
    {
        if (_enemyTarget == null || !_agent.enabled)
            return;

        _agent.destination = _enemyTarget.gameObject.transform.position;
    }

    public float GetMovementSpeed()
    {
        return _movementSpeed;
    }
    public float GetRotationSpeed()
    {
        return _rotationSpeed;
    }
    public float GetStopDistance()
    {
        return _stopDistance;
    }
    public float GetSqrDestinationDistance()
    {
        return (transform.position - _agent.destination).sqrMagnitude;
    }
    public void SwitchAgentActivityState(bool isStopped)
    {
        if (gameObject.activeSelf && _agent.enabled)
            _agent.isStopped = isStopped;
    }
}

public interface IGetMovementSpeed
{
    public float GetMovementSpeed();
}

public interface IGetRotationSpeed
{
    public float GetRotationSpeed();
}

public interface IGetStopDistance
{
    public float GetStopDistance();
}

public interface IGetSqrDestinationDistance
{
    public float GetSqrDestinationDistance();
}

public interface ISwitchAgentActivityState
{
    public void SwitchAgentActivityState(bool isStopped);
}