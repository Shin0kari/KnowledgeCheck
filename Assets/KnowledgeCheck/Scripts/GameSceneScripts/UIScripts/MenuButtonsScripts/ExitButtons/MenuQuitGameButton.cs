using Cysharp.Threading.Tasks;

public class MenuQuitGameButton : AbstractMenuExitButton
{
    protected override async UniTask AsyncSetLisnener()
    {
        await base.AsyncSetLisnener();

        _button.onClick.AddListener(() =>
        {
            _backgroundDeniedPanel.SetActive(true);
            _quitButtonPressedChecker.OnQuitGameButton();
            _quitWarningPanel.SetActive(true);
        });
    }
}