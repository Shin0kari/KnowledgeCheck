using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class ScrollNewGameButton : UIButton, IChangeButtonVisible
{
    // [SerializeField] private Button _button;

    // private void Start()
    // {
    //     _button.onClick.AddListener(() =>
    //     {
    //         NewSave();
    //     });
    // }

    // public void NewSave()
    // {
    //     Debug.Log("[1_ScrollNewGameButton]: NewSave Start");
    //     Debug.Log($"[1_ScrollNewGameButton]: This button has {IsUsed?.GetInvocationList().Count()} Subscribes");
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