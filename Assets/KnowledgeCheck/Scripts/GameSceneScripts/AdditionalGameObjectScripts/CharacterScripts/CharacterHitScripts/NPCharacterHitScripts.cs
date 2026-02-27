using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(IGetStopDistance)), RequireComponent(typeof(IGetSqrDestinationDistance)), RequireComponent(typeof(ISwitchAgentActivityState))]
public class NPCharacterHitScripts : HitScipts, IAttackSignalSender
{
    private const float MIN_ATTACK_DELAY = 1.5f;

    private IGetStopDistance _stopDistanceSender;
    private IGetSqrDestinationDistance _sqrDestinationDistanceSender;
    private ISwitchAgentActivityState _agentActivityStateSwitch;

    private float _attackDistance;
    private float _sqrDestinationDistance;

    public event Action OnStartAttack;
    public event Action OnEndAttack;

    private void Awake()
    {
        _stopDistanceSender = GetComponent<IGetStopDistance>();
        _sqrDestinationDistanceSender = GetComponent<IGetSqrDestinationDistance>();
        _agentActivityStateSwitch = GetComponent<ISwitchAgentActivityState>();

        _attackDistance = _stopDistanceSender.GetStopDistance();
    }

    private void Start()
    {
        AsyncTryAttack(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask AsyncTryAttack(CancellationToken token)
    {
        while (true)
        {
            await UniTask.WaitForSeconds(MIN_ATTACK_DELAY, cancellationToken: token);

            _sqrDestinationDistance = _sqrDestinationDistanceSender.GetSqrDestinationDistance();
            if (_sqrDestinationDistance <= _attackDistance * _attackDistance)
                Attack();

            await UniTask.Yield(token);
        }
    }

    protected override void Attack()
    {
        if (_isAttackEnded)
        {
            OnStartAttack?.Invoke();
            _agentActivityStateSwitch.SwitchAgentActivityState(true);

            _characterAnimation.SetCharacterHitNoMoveAnim();
            _isAttackEnded = false;
        }
    }
    private void OnDisable()
    {
        if (!_isAttackEnded)
            EndAttack();
    }
    public override void EndAttack()
    {
        OnEndAttack?.Invoke();
        _agentActivityStateSwitch.SwitchAgentActivityState(false);
        _isAttackEnded = true;
    }
}