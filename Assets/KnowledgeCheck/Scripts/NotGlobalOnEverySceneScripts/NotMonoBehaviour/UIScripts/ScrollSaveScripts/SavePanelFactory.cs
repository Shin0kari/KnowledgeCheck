using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SavePanelFactory
{
    // private List<SavePanel> _instantiatedPanels = new();

    private SavePanel _savePanel;
    private SavePanel.Factory _savePanelFactory;
    private DeleteSavePanel _deleteSavePanel;

    [Inject]
    private void Construct(
        SavePanel.Factory savePanelFactory,
        DeleteSavePanel deleteSavePanel)
    {
        _savePanelFactory = savePanelFactory;
        _deleteSavePanel = deleteSavePanel;
    }

    public GameObject InstantiateSave(IScrollUtils scrollsUtils)
    {
        _savePanel = _savePanelFactory.Create(scrollsUtils.GetSavePrefab());
        _savePanel.transform.SetParent(scrollsUtils.GetScroll().content);
        _savePanel.transform.localScale = new(1f, 1f, 1f);

        _savePanel.GetDeleteSaveButton().SetDeleteSavePanel(_deleteSavePanel);

        return _savePanel.gameObject;
    }

    public void DestroyInstanceOnScroll(GameObject savePanelObject)
    {
        UnityEngine.Object.Destroy(savePanelObject);
    }

    // public void Initialize()
    // {
    //     Debug.Log("[3_SavePanelFactory]: Init");
    // }

    // public void Dispose()
    // {
    //     Debug.Log("[3_SavePanelFactory]: Disposable");
    // }
}