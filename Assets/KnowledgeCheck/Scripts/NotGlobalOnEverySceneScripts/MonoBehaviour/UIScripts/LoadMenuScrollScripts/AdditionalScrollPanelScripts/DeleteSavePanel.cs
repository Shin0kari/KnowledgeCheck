using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeleteSavePanel : MonoBehaviour
{
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _deniedButton;

    [SerializeField] private Button _backgroundButton;
    [SerializeField] private GameObject _deleteSavePanel;

    private SavePanel _savePanel;

    private GameDataChanger _gameDataChanger;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger)
    {
        _gameDataChanger = gameDataChanger;
    }

    private void Start()
    {
        _acceptButton.onClick.AddListener(() =>
        {
            ClosePanel();
            DeleteGameData();
        });
        _deniedButton.onClick.AddListener(() =>
        {
            ClosePanel();
        });
    }

    private void DeleteGameData()
    {
        _gameDataChanger.DeleteSave(_savePanel.GetSaveName());
    }

    private void ClosePanel()
    {
        _deleteSavePanel.SetActive(false);
    }

    public void SetDeletedSavePanel(SavePanel savePanel)
    {
        _savePanel = savePanel;
        _deleteSavePanel.SetActive(true);
    }

    // private void OnDestroy()
    // {
    // }
}