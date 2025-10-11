using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LoadChoicedSaveButton : MonoBehaviour, IButton
{
    [SerializeField] private Button _button;
    [SerializeField] private SavePanel _savePanel;

    private GameDataChanger _gameDataChanger;

    public event Action IsUsed;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger)
    {
        _gameDataChanger = gameDataChanger;
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            LoadChoicedSave();
        });
    }

    public void LoadChoicedSave()
    {
        _gameDataChanger.ChangeCurrentSave(_savePanel.GetSaveName());
        IsUsed?.Invoke();
    }
}