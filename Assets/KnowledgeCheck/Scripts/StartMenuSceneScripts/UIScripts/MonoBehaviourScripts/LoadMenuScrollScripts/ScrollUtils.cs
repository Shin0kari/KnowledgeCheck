using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScrollUtils : MonoBehaviour, IScrollUtils
{
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private GameObject _newSavePrefab;
    [SerializeField] private GameObject _newSaveButton;

    private DiContainer _container;
    private IButtonRegistry _buttonRegistry;
    private DeleteSavePanel _deleteSavePanel;

    [Inject]
    private void Construct(DiContainer container, IButtonRegistry buttonRegistry, DeleteSavePanel deleteSavePanel)
    {
        _container = container;
        _buttonRegistry = buttonRegistry;
        _deleteSavePanel = deleteSavePanel;
    }

    public GameObject InstantiateSaveToScroll()
    {
        var parentContent = _scroll.content;

        var savePanelObject = _container.InstantiatePrefab(_newSavePrefab, parentContent);
        var savePanel = savePanelObject.GetComponent<SavePanel>();
        Debug.Log("[SCROLL_UTILS]: Insantiate savePanelObject.");
        savePanel.GetDeleteSaveButton().SetDeleteSavePanel(_deleteSavePanel);
        Debug.Log("[SCROLL_UTILS]: Register LoadChoiceButton.");
        _buttonRegistry.Register(savePanel.GetLoadChoiceButton());
        return savePanelObject;
    }

    public void DestroyInstanceOnScroll(GameObject savePanel)
    {
        _buttonRegistry.Unregister(savePanel.GetComponent<SavePanel>().GetLoadChoiceButton());
        Destroy(savePanel);
    }

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
    public GameObject GetSavePrefab() => _newSavePrefab;
    public GameObject GetNewSaveButton() => _newSaveButton;

}