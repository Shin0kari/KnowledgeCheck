using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallChecker : MonoBehaviour
{
    private const float MIN_FALL_SPEED = -3f;
    private const float DURATION_BETWEEN_CHECK = 0.5f;
    private bool _isFall = false;
    private Rigidbody _rigidbody;

    public event Action FallStarted;
    public event Action LandStarted;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        AsyncCheckGrounded(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid AsyncCheckGrounded(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (_rigidbody.linearVelocity.y < MIN_FALL_SPEED && !_isFall)
                SendFallSignal();
            else if (_rigidbody.linearVelocity.y >= MIN_FALL_SPEED && _isFall)
                SendLandSignal();

            await UniTask.WaitForSeconds(DURATION_BETWEEN_CHECK, cancellationToken: cancellationToken);
        }
    }

    private void SendFallSignal()
    {
        _isFall = true;
        FallStarted?.Invoke();
    }

    private void SendLandSignal()
    {
        _isFall = false;
        LandStarted?.Invoke();
    }
}