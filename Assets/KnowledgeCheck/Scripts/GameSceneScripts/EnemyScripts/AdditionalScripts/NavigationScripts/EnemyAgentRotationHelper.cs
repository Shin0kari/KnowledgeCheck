using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(IAttackSignalSender))]
[RequireComponent(typeof(IGetStopDistance))]
[RequireComponent(typeof(IGetSqrDestinationDistance))]
public class EnemyAgentRotationHelper : MonoBehaviour, IDisposable
{
    private NavMeshAgent _agent;
    private IAttackSignalSender _attackSignalSender;
    private IGetStopDistance _stopDistanceSender;
    private IGetSqrDestinationDistance _sqrDestinationDistanceSender;

    private float _attackDistance;
    private float _sqrDestinationDistance;
    private float _rotationSpeed;
    private bool _isAttack;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _attackSignalSender = GetComponent<IAttackSignalSender>();
        _stopDistanceSender = GetComponent<IGetStopDistance>();
        _sqrDestinationDistanceSender = GetComponent<IGetSqrDestinationDistance>();

        _attackDistance = _stopDistanceSender.GetStopDistance();

        _rotationSpeed = _agent.angularSpeed;

        _attackSignalSender.OnStartAttack += OnStartAttack;
        _attackSignalSender.OnEndAttack += OnEndAttack;
    }

    public void Dispose()
    {
        if (_attackSignalSender != null)
        {
            _attackSignalSender.OnStartAttack -= OnStartAttack;
            _attackSignalSender.OnEndAttack -= OnEndAttack;
        }
    }

    private void OnStartAttack()
    {
        ChangeRotationHelperAttackState(true);
    }

    private void OnEndAttack()
    {
        ChangeRotationHelperAttackState(false);
    }

    private void ChangeRotationHelperAttackState(bool state)
    {
        _isAttack = state;
    }

    private void Update()
    {
        if (!_agent.enabled || _isAttack)
            return;

        _sqrDestinationDistance = _sqrDestinationDistanceSender.GetSqrDestinationDistance();
        if (_sqrDestinationDistance > _attackDistance * _attackDistance)
            return;

        Vector3 direction = _agent.destination - transform.position;
        direction.y = 0;

        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );
    }
}