using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ArenaGateController : MonoBehaviour
{
    [SerializeField] private Animator _gateAnimator;
    // [SerializeField] private float _heightOffset = -4.75f;
    // [SerializeField] private float _startYPos = 0f;
    // [SerializeField] private float _animationDuration = 2f;
    // [SerializeField] private Ease _easeAnimation = Ease.InQuart;

    private CancellationToken _cancellationToken;

    private void Awake()
    {
        _cancellationToken = gameObject.GetCancellationTokenOnDestroy();
    }

    private void Start()
    {
        OpenGate();
    }

    public void OpenGate()
    {
        _gateAnimator.SetTrigger("OpenGate");
        // transform
        //     .DOLocalMoveY(transform.position.y + _heightOffset, _animationDuration)
        //     .SetEase(_easeAnimation)
        //     .WithCancellation(_cancellationToken);
    }

    public void CloseGate()
    {
        _gateAnimator.SetTrigger("CloseGate");
    }
}