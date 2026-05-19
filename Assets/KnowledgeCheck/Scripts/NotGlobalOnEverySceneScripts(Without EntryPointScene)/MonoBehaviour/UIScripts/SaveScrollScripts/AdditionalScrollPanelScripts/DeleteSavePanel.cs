using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeleteSavePanel : MonoBehaviour, IBindingSingletonComponent
{
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _deniedButton;

    // [SerializeField] private Button _backgroundButton;
    private GameObject _savePanelOnDelete;

    private SavePanel _savePanel;

    private GameDataChanger _gameDataChanger;

    [Inject]
    private void Construct(GameDataChanger gameDataChanger)
    {
        _gameDataChanger = gameDataChanger;

        _savePanelOnDelete = gameObject;

        BindAllTypes();
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
        if (_savePanel == null) return;

        _gameDataChanger.DeleteSave(_savePanel.GetSaveUuid());
    }

    private void ClosePanel()
    {
        _savePanelOnDelete.SetActive(false);
    }

    public void SetSavePanelOnDelete(SavePanel savePanel)
    {
        _savePanel = savePanel;
        _savePanelOnDelete.SetActive(true);
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}