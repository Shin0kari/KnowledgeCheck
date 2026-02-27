using UnityEngine;
using UnityEngine.EventSystems;

public class CheckClickOnUIButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string _panelName;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Click on Button - {_panelName}");
    }
}