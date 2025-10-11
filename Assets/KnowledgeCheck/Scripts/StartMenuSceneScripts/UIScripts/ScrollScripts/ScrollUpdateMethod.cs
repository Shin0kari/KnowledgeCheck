using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScrollUpdateMethod : IUpdateScroll
{
    private IScrollUtils _scrollUtils;

    [Inject]
    private void Construct(IScrollUtils scrollUtils)
    {
        _scrollUtils = scrollUtils;
    }

    public List<GameObject> CreateAllSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        List<GameObject> createdSaves = new();

        try
        {
            DeleteSaves();

            foreach (var save in saves)
            {
                var updatedSave = AddSave(save.Value);
                createdSaves.Add(updatedSave);
            }
        }
        catch (System.Exception)
        {

            throw;
        }
        return createdSaves;
    }

    public GameObject AddSave(SaveData saveData)
    {
        GameObject newSave = null;

        try
        {
            Debug.Log("[SCROLL_UPDATE_METHOD]: start spawn newSavePanel.");
            newSave = _scrollUtils.InstantiateSaveToScroll();
            Debug.Log("[SCROLL_UPDATE_METHOD]: newSavePanel is spawned.");

            var savePanelData = newSave.GetComponent<SavePanel>();
            savePanelData.SetSaveName(saveData.SaveName);
        }
        catch (System.Exception)
        {
            _scrollUtils.DestroyInstanceOnScroll(newSave);
        }
        return newSave;
    }

    public void DeleteMissingSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        foreach (var content in _scrollUtils.GetAllContent())
        {
            if (!saves.ContainsKey(content.GetComponent<SavePanel>().GetSaveName()))
                _scrollUtils.DestroyInstanceOnScroll(content);
        }
    }

    public GameObject UpdateCurrentSave((string newSaveName, SaveData saveData) currentSave)
    {
        GameObject updatedSave = null;
        try
        {
            foreach (var content in _scrollUtils.GetAllContent())
            {
                var savePanel = content.GetComponent<SavePanel>();

                if (currentSave.saveData.SaveName == savePanel.GetSaveName())
                {
                    savePanel.SetSaveName(currentSave.newSaveName);
                    updatedSave = content;
                    break;
                }
            }
        }
        catch (System.Exception)
        {
            throw;
        }
        return updatedSave;
    }

    public List<GameObject> UpdateAllSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        return CreateAllSaves(saves);
    }

    private void DeleteSaves()
    {
        if (_scrollUtils.GetCountContent() > 1)
        {
            for (int i = 1; i < _scrollUtils.GetCountContent(); i++)
            {
                _scrollUtils.DestroyInstanceOnScroll(_scrollUtils.GetScrollChildGameObject(i));
            }
        }

        if (_scrollUtils.GetCountContent() < 5)
        {
            _scrollUtils.SetActiveStateForNewSaveButton(true);
        }
    }

}
