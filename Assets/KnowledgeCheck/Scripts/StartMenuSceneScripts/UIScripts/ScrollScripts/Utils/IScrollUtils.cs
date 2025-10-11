using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IScrollUtils
{
    public List<GameObject> GetAllContent();
    public int GetCountSaves();
    public int GetCountContent();
    public GameObject GetNewSaveButton();
    public GameObject GetSavePrefab();
    public ScrollRect GetScroll();

    public void SetActiveStateForNewSaveButton(bool state);
    public GameObject GetScrollChildGameObject(int childIndex);

    public GameObject InstantiateSaveToScroll();
    public void DestroyInstanceOnScroll(GameObject save);
}
