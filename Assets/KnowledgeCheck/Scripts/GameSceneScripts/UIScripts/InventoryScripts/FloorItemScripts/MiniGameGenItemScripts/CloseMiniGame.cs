using System;
using UnityEngine;
using UnityEngine.UI;

public class CloseMiniGame : MonoBehaviour
{
    [SerializeField] Button _button;

    [SerializeField] private ButtonItemGenerator _buttonItemGenerator;
    [SerializeField] private HoldMiniGameButton _miniGame;
    [SerializeField] private GameObject _backgroundMiniGame;

    private void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            StopMiniGame();
        });
    }

    private void StopMiniGame()
    {
        _buttonItemGenerator.EnableButton();
        _backgroundMiniGame.SetActive(false);
        _miniGame.gameObject.SetActive(false);
    }
}