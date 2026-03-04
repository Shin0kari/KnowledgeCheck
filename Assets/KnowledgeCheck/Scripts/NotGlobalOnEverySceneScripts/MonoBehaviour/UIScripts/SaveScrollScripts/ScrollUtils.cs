using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScrollUtils : MonoBehaviour, IScrollUtils
{
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private GameObject _savePanelPrefab;
    [SerializeField] private GameObject _newSaveButton;

    public void SetActiveStateForNewSaveButton(bool state)
    {
        _newSaveButton.SetActive(state);
    }

    public GameObject GetScrollChildGameObject(int childIndex) => _scroll.content.GetChild(childIndex).gameObject;

    public int GetCountSaves()
    {
        return _newSaveButton.activeSelf ?
            _scroll.content.childCount - 1 :
            _scroll.content.childCount;
    }

    public int GetCountContent() => _scroll.content.childCount;

    public List<GameObject> GetAllContent()
    {
        List<GameObject> allContent = new();

        for (int i = 1; i < GetCountContent(); i++)
        {
            allContent.Add(_scroll.content.GetChild(i).gameObject);
        }

        return allContent;
    }

    public ScrollRect GetScroll() => _scroll;
    public GameObject GetSavePrefab() => _savePanelPrefab;
    public GameObject GetNewSaveButton() => _newSaveButton;

    // private void OnDestroy()
    // {
    // }
}