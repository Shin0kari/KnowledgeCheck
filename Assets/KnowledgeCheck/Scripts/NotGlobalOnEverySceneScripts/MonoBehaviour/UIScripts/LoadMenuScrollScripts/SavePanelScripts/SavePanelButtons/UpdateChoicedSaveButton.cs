using System;
using UnityEngine;
using UnityEngine.UI;
public class UpdateChoicedSaveButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _changerPanel;

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            _changerPanel.SetActive(true);
        });
    }
}