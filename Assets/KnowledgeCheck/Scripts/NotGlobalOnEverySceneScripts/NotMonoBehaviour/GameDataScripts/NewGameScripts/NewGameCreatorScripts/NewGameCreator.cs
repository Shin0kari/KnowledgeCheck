using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NewGameCreator : IDisposable
{
    // private List<IButton> _buttonsList;

    private NewGame _newGame;
    private NewSave _newSave;
    private ContinueGame _continueGame;
    private LoadGameData _loadGameData;
    private IButtonRegistry _buttonRegistry;

    [Inject]
    private void Construct(
        NewGame newGame,
        NewSave newSave,
        ContinueGame continueGame,
        LoadGameData loadGameData,
        IButtonRegistry buttonRegistry)
    {
        _newGame = newGame;
        _newSave = newSave;
        _continueGame = continueGame;
        _loadGameData = loadGameData;
        _buttonRegistry = buttonRegistry;

        // _buttonsList = new();

        // foreach (var button in _buttonRegistry.GetButtons())
        //     Subscribe(button);

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

            case ScrollNewSaveButton:
                ToggleSubscription(button, subscribe, OnNewSave);
                break;

            case ContinueGameButton:
                ToggleSubscription(button, subscribe, OnContinueGame);
                break;

            case SaveDataLoaderButton or LoadLastSaveButton:
                ToggleSubscription(button, subscribe, OnLoadGameData);
                break;

                // case SaveDataSaverButton:
                //     ToggleSubscription(button, subscribe, OnSaveGameData);
                //     break;
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

    private void OnContinueGame()
    {
        _continueGame.StartProcess();
    }

    private void OnNewSave()
    {
        _newSave.StartProcess();
    }

    private void OnLoadGameData()
    {
        _loadGameData.StartProcess();
    }

    public void Dispose()
    {
        _buttonRegistry.ButtonAdded -= Subscribe;
        _buttonRegistry.ButtonRemoved -= Unsubscribe;
    }
}
