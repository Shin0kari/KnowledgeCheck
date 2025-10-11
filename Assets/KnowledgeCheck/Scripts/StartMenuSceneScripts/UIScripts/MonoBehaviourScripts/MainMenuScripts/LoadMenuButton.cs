using System;
using UnityEngine;
using UnityEngine.UI;
public class LoadMenuButton : MonoBehaviour, IChangeButtonInteractable
{
    [SerializeField] private Button _button;

    public void DisableButton()
    {
        _button.interactable = false;
    }

    public void EnableButton()
    {
        _button.interactable = true;
    }
}