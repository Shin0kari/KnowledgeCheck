using System;
using UnityEngine;
using UnityEngine.UI;
public class ContinueGameButton : MonoBehaviour, IButton, IChangeButtonVisible
{
    [SerializeField] private Button _button;

    public event Action IsUsed;

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            ContinueGame();
        });
    }

    public void ContinueGame()
    {
        IsUsed?.Invoke();
    }

    public void HideButton()
    {
        _button.gameObject.SetActive(false);
    }

    public void RevealButton()
    {
        _button.gameObject.SetActive(true);
    }
}
