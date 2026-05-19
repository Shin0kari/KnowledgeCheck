using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ArenaGateController : MonoBehaviour, IBindingSingletonComponent
{
    [SerializeField] private Animator _gateAnimator;

    private void Awake()
    {
        BindAllTypes();
    }

    private void Start()
    {
        OpenGate();
    }

    public void OpenGate()
    {
        _gateAnimator.SetTrigger("OpenGate");
    }

    public void CloseGate()
    {
        _gateAnimator.SetTrigger("CloseGate");
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}