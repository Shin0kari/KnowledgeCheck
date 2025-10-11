using System;
using UnityEngine;
using Zenject;

public class CheckButtonAvailabilty
{
    private IGetGameData _gameData;

    private NewGameButton _newGameButton;
    private ContinueGameButton _continueButton;
    private NewSaveButton _newSaveButton;
    private LoadMenuButton _loadGameButton;

    private SaveChecker _saveChecker;

    [Inject]
    private void Construct(
        IGetGameData gameData,
        [Inject(Id = "NewGameButton")] IButton newGameButton,
        [Inject(Id = "ContinueGameButton")] IButton continueButton,
        [Inject(Id = "NewSaveButton")] IButton newSaveButton,
        LoadMenuButton loadGameButton,
        SaveChecker saveChecker)
    {
        _gameData = gameData;
        _newGameButton = newGameButton as NewGameButton;
        _continueButton = continueButton as ContinueGameButton;
        _newSaveButton = newSaveButton as NewSaveButton;
        _loadGameButton = loadGameButton;
        _saveChecker = saveChecker;

        _saveChecker.IsCountDataChanged += CheckNewGameButton;
        _saveChecker.IsCountDataChanged += CheckContinueGameButton;
        _saveChecker.IsCountDataChanged += CheckNewSaveButton;
        _saveChecker.IsCountDataChanged += CheckLoadGameButton;
    }

    private void CheckNewGameButton()
    {
        if (_gameData.GetAllGameDatas().Count > 0)
            _newGameButton.DisableButton();
        else
            _newGameButton.EnableButton();
    }

    private void CheckContinueGameButton()
    {
        if (_gameData.GetCurrentGameData().saveData == null)
            _continueButton.HideButton();
        else
            _continueButton.RevealButton();
    }

    private void CheckNewSaveButton()
    {
        if (_gameData.GetAllGameDatas().Count > 3)
            _newSaveButton.HideButton();
        else
            _newSaveButton.RevealButton();
    }

    private void CheckLoadGameButton()
    {
        if (_gameData.GetAllGameDatas().Count < 1)
            _loadGameButton.DisableButton();
        else
            _loadGameButton.EnableButton();
    }
}