using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NewGameCreator : IDisposable
{
    // private List<IButton> _buttonsList;

    private NewGame _newGame;
    private ContinueGame _continueGame;
    private LoadGameData _loadGameData;
    private IButtonRegistry _buttonRegistry;

    [Inject]
    private void Construct(
        NewGame newGame,
        ContinueGame continueGame,
        LoadGameData loadGameData,
        IButtonRegistry buttonRegistry)
    {
        _newGame = newGame;
        _continueGame = continueGame;
        _loadGameData = loadGameData;
        _buttonRegistry = buttonRegistry;

        _buttonRegistry.ButtonAdded += Subscribe;
        _buttonRegistry.ButtonRemoved += Unsubscribe;
    }

    private void Subscribe(UIButton button) => HandleSubscription(button, true);
    private void Unsubscribe(UIButton button) => HandleSubscription(button, false);

    private void HandleSubscription(UIButton button, bool subscribe)
    {
        switch (button)
        {
            case NewGameButton or ScrollNewGameButton:
                ToggleSubscription(button, subscribe, OnNewGame);
                break;

            case ContinueGameButton or LoadLastSaveButton:
                ToggleSubscription(button, subscribe, OnLoadLastGame);
                break;

            case SaveDataLoaderButton:
                ToggleSubscription(button, subscribe, OnLoadGameData);
                break;
        }
    }

    private void ToggleSubscription<T>(T button, bool subscribe, Action action) where T : UIButton
    {
        if (subscribe)
        {
            button.IsUsed += action;
        }
        else
        {
            button.IsUsed -= action;
        }
    }

    private void OnNewGame()
    {
        _newGame.StartProcess();
    }

    private void OnLoadLastGame()
    {
        _continueGame.StartProcess();
    }

    private void OnLoadGameData()
    {
        _loadGameData.StartProcess();
    }

    public void Dispose()
    {
        if (_buttonRegistry != null)
        {
            _buttonRegistry.ButtonAdded -= Subscribe;
            _buttonRegistry.ButtonRemoved -= Unsubscribe;
        }
    }
}
