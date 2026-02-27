using UnityEngine;

public class QuitButtonPressedChecker : MonoBehaviour
{
    [SerializeField] private QuitGameButton _exitToMenuButton;
    [SerializeField] private QuitGameButton _quitGameButton;

    public void OnExitToMenuButton()
    {
        _exitToMenuButton.enabled = true;
        _quitGameButton.enabled = false;
    }

    public void OnQuitGameButton()
    {
        _exitToMenuButton.enabled = false;
        _quitGameButton.enabled = true;
    }
}