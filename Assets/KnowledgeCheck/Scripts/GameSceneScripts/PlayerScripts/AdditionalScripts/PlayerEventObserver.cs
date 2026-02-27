using System;
using Zenject;

// Используется, чтобы выделить среди CharacterEventObserver тот, который принадлежит игроку
public class PlayerEventObserver : CharacterEventObserver
{
    private ArenaController _arenaController;

    [Inject]
    private void Construct(ArenaController arenaController)
    {
        _arenaController = arenaController;
    }

    public override void SetDeathState()
    {
        base.SetDeathState();
        _arenaController.OnPlayerDeath();
    }
}