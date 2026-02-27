using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScrollUpdateMethod : IUpdateScroll
{
    private SavePanelFactory _savePanelFactory;
    private IScrollUtils[] _scrollsUtils;

    [Inject]
    private void Construct(
        SavePanelFactory savePanelFactory,
        IScrollUtils[] scrollsUtils)
    {
        _savePanelFactory = savePanelFactory;
        _scrollsUtils = scrollsUtils;
    }

    public void CreateAllSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        try
        {
            DeleteSaves();

            foreach (var save in saves)
            {
                AddSave(save.Value);
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public void AddSave(SaveData saveData)
    {
        List<GameObject> newSaves = new();
        try
        {
            foreach (var newSave in InstantiateSaveToScroll())
            {
                var savePanelData = newSave.GetComponent<SavePanel>();
                savePanelData.SetSaveName(saveData.SaveName);
                newSaves.Add(newSave);
            }
        }
        catch (System.Exception)
        {
            foreach (var newSave in newSaves)
            {
                _savePanelFactory.DestroyInstanceOnScroll(newSave);
            }
        }
    }

    private List<GameObject> InstantiateSaveToScroll()
    {
        List<GameObject> savePanels = new();

        foreach (var scrollsUtils in _scrollsUtils)
        {
            var savePanelObject = _savePanelFactory.InstantiateSave(scrollsUtils);
            savePanels.Add(savePanelObject);
        }
        return savePanels;
    }

    public void DeleteMissingSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        foreach (var scrollUtils in _scrollsUtils)
        {
            foreach (var content in scrollUtils.GetAllContent())
            {
                if (!saves.ContainsKey(content.GetComponent<SavePanel>().GetSaveName()))
                    _savePanelFactory.DestroyInstanceOnScroll(content);
            }
        }
    }

    public void UpdateCurrentSave((string newSaveName, SaveData saveData) currentSave)
    {
        try
        {
            foreach (var scrollUtils in _scrollsUtils)
            {
                foreach (var content in scrollUtils.GetAllContent())
                {
                    var savePanel = content.GetComponent<SavePanel>();

                    if (currentSave.saveData.SaveName == savePanel.GetSaveName())
                    {
                        savePanel.SetSaveName(currentSave.newSaveName);
                        break;
                    }
                }
            }
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public void UpdateAllSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        CreateAllSaves(saves);
    }

    private void DeleteSaves()
    {
        foreach (var scrollUtils in _scrollsUtils)
        {
            if (scrollUtils.GetCountContent() > 1)
            {
                for (int i = 1; i < scrollUtils.GetCountContent(); i++)
                {
                    _savePanelFactory.DestroyInstanceOnScroll(scrollUtils.GetScrollChildGameObject(i));
                }
            }

            if (scrollUtils.GetCountContent() < 5)
            {
                scrollUtils.SetActiveStateForNewSaveButton(true);
            }
        }
    }

    // public void Dispose()
    // {
    //     Debug.Log("[2_ScrollUpdateMethods]: Disposable");
    // }

    // public void Initialize()
    // {
    //     Debug.Log("[2_ScrollUpdateMethods]: Init");
    // }
}
