using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class ActorInteractionAudioRepository : IDisposable
{
    private ActorInteractionAudioProvider _actorInteractionAudioProvider;

    private ReactiveProperty<ScriptableObject> _reactiveActorInteractionAudioSO;
    private ActorInteractionAudioSO _actorInteractionAudioSO;

    private DisposableBag _dB;
    private UniTaskCompletionSource _actorInteractionAudioSOLoadedSource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(ActorInteractionAudioProvider actorInteractionAudioProvider)
    {
        _actorInteractionAudioProvider = actorInteractionAudioProvider;

        AsyncLoadResource().Forget();
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();
        _ct = null;

        _dB.Dispose();
    }

    private async UniTask AsyncLoadResource()
    {
        _reactiveActorInteractionAudioSO = await _actorInteractionAudioProvider.TryGetDataSO(_ct.Token);

        _reactiveActorInteractionAudioSO?
            .Subscribe(ActorInteractionAudioSO =>
            {
                if (ActorInteractionAudioSO == null)
                    return;

                SetSO(ActorInteractionAudioSO).Forget();
            })
            .AddTo(ref _dB);
    }

    private async UniTask SetSO(ScriptableObject ActorInteractionAudioSO)
    {
        if (ActorInteractionAudioSO is not ActorInteractionAudioSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        _actorInteractionAudioSO = so;

        _actorInteractionAudioSO.SetDefaultState();
        await _actorInteractionAudioSO.GroupAllSounds(_ct.Token);
        _actorInteractionAudioSOLoadedSource.TrySetResult();
    }

    public async UniTask<ActorInteractionAudioSO> AsyncGetActorInteractionAudioSO(CancellationToken ct)
    {
        var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(
            _ct.Token,
            ct
        );
        await _actorInteractionAudioSOLoadedSource.Task.AttachExternalCancellation(linkedCTS.Token);

        return _actorInteractionAudioSO;
    }
}