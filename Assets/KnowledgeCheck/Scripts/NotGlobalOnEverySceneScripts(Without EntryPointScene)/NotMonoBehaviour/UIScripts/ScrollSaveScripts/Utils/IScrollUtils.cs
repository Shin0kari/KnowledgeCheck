using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public interface IScrollUtils : IBindingTransientComponent
{
    public List<GameObject> GetAllContent();
    public int GetCountSaves();
    public int GetCountContent();
    public UniTask<GameObject> GetNewSaveButton(CancellationToken ct);
    public UniTask<GameObject> GetSavePrefab(CancellationToken ct);
    public ScrollRect GetScroll();

    public void SetActiveStateForNewSaveButton(bool state);
    public GameObject GetScrollChildGameObject(int childIndex);
}
