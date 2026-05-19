using UnityEngine;
using UnityEngine.EventSystems;

public class CheckClickOnUIButton : MonoBehaviour, IPointerClickHandler
{
    private string _panelName;
    private void Awake()
    {
        _panelName = this.name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Click on Button - {_panelName}");
    }
}