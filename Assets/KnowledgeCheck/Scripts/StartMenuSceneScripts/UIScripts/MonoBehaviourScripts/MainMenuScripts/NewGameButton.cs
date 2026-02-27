using System;
using UnityEngine;
using UnityEngine.UI;
public class NewGameButton : UIButton, IChangeButtonInteractable
{
    public void DisableButton()
    {
        _button.interactable = false;
    }

    public void EnableButton()
    {
        _button.interactable = true;
    }
}
