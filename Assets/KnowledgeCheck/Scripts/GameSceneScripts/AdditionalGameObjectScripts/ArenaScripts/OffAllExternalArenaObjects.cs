using System;
using Zenject;

public class OffAllExternalArenaObjects : IStateController, IDisposable
{
    private ArenaController _arenaController;

    public event Action OnOnState;
    public event Action OnOffState;

    [Inject]
    private void Construct(ArenaController arenaController)
    {
        _arenaController = arenaController;

        _arenaController.StartArenaBattle += OnBattleStarted;
    }

    private void OnBattleStarted()
    {
        OnOffState?.Invoke();
    }

    public void Dispose()
    {
        _arenaController.StartArenaBattle -= OnBattleStarted;
    }
}

public interface IStateController
{
    public event Action OnOnState;
    public event Action OnOffState;
}