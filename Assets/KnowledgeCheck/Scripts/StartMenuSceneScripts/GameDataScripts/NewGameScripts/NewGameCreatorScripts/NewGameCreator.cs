using System;
using UnityEngine;
using Zenject;

public class NewGameCreator
{
    private NewGame _newGame;
    private ContinueGame _continueGame;
    private NewSave _newSave;
    private LoadSave _loadSave;
    private IButtonRegistry _buttonRegistry;

    [Inject]
    private void Construct(
        NewGame newGame,
        ContinueGame continueGame,
        NewSave newSave,
        LoadSave loadSave,
        IButtonRegistry buttonRegistry)
    {
        _newGame = newGame;
        _continueGame = continueGame;
        _newSave = newSave;
        _loadSave = loadSave;
        _buttonRegistry = buttonRegistry;

        foreach (var button in _buttonRegistry.GetButtons())
            Subscribe(button);

        _buttonRegistry.ButtonAdded += Subscribe;
        _buttonRegistry.ButtonRemoved += Unsubscribe;
    }

    private void Subscribe(IButton button) => HandleSubscription(button, true);
    private void Unsubscribe(IButton button) => HandleSubscription(button, false);

    private void HandleSubscription(IButton button, bool subscribe)
    {
        switch (button)
        {
            case NewGameButton btn:
                if (subscribe) btn.IsUsed += OnNewGame; else btn.IsUsed -= OnNewGame;
                break;

            case ContinueGameButton btn:
                if (subscribe) btn.IsUsed += OnContinueGame; else btn.IsUsed -= OnContinueGame;
                break;

            case NewSaveButton btn:
                if (subscribe) btn.IsUsed += OnNewSave; else btn.IsUsed -= OnNewSave;
                break;

            case LoadChoicedSaveButton btn:
                Debug.Log("[NEW_GAME_CREATOR]: подписка для LoadChoicedSaveButton сделана.");
                if (subscribe) btn.IsUsed += OnLoadSave; else btn.IsUsed -= OnLoadSave;
                break;
        }
    }

    private void OnNewGame()
    {
        _newGame.StartProcess();
    }

    private void OnContinueGame()
    {
        _continueGame.StartProcess();
    }

    private void OnNewSave()
    {
        _newSave.StartProcess();
    }

    private void OnLoadSave()
    {
        _loadSave.StartProcess();
    }
}
