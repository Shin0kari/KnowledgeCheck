using System;
using UnityEngine;
using UnityEngine.UI;
public class NewGameButton : MonoBehaviour, IButton, IChangeButtonInteractable
{
    [SerializeField] private Button _button;

    public event Action IsUsed;

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            NewGame();
        });
    }

    public void NewGame()
    {
        IsUsed?.Invoke();
    }

    public void DisableButton()
    {
        _button.interactable = false;
    }

    public void EnableButton()
    {
        _button.interactable = true;
    }
}
