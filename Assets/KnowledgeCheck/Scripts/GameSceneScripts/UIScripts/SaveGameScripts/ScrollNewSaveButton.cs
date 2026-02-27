using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class ScrollNewSaveButton : UIButton, IChangeButtonVisible
{

    // private void Start()
    // {
    //     _button.onClick.AddListener(() =>
    //     {
    //         NewSave();
    //     });
    // }

    // public void NewSave()
    // {
    //     IsUsed?.Invoke();
    // }

    public void HideButton()
    {
        _button.gameObject.SetActive(false);
    }

    public void RevealButton()
    {
        _button.gameObject.SetActive(true);
    }
}