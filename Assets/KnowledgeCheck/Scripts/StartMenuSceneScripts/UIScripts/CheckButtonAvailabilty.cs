using System;
using UnityEngine;
using Zenject;

public class CheckButtonAvailabilty
{
    private IGetGameData _gameData;

    private NewGameButton _newGameButton;
    private ContinueGameButton _continueButton;
    private ScrollNewGameButton _scrollNewGameButton;
    private LoadMenuButton _loadGameButton;

    private SaveChecker _saveChecker;

    [Inject]
    private void Construct(
        IGetGameData gameData,
        NewGameButton newGameButton,
        ContinueGameButton continueButton,
        ScrollNewGameButton scrollNewGameButton,
        LoadMenuButton loadGameButton,
        SaveChecker saveChecker)
    {
        _gameData = gameData;
        _newGameButton = newGameButton;
        _continueButton = continueButton;
        _scrollNewGameButton = scrollNewGameButton;
        _loadGameButton = loadGameButton;
        _saveChecker = saveChecker;

        _saveChecker.IsCountDataChanged += CheckNewGameButton;
        _saveChecker.IsCountDataChanged += CheckContinueGameButton;
        _saveChecker.IsCountDataChanged += CheckNewSaveButton;
        _saveChecker.IsCountDataChanged += CheckLoadGameButton;
    }

    private void CheckNewGameButton()
    {
        // if (_gameData.GetAllGameDatas().Count > 0)
        //     _newGameButton.DisableButton();
        // else
        //     _newGameButton.EnableButton();
    }

    private void CheckContinueGameButton()
    {
        if (_gameData.GetCurrentGameData().saveData == null)
            _continueButton.HideButton();
        // else
        //     _continueButton.RevealButton();
    }

    private void CheckNewSaveButton()
    {
        if (_gameData.GetAllGameDatas().Count > 3)
            _scrollNewGameButton.HideButton();
        else
            _scrollNewGameButton.RevealButton();
    }

    private void CheckLoadGameButton()
    {
        if (_gameData.GetAllGameDatas().Count < 1)
            _loadGameButton.DisableButton();
        else
            _loadGameButton.EnableButton();
    }
}