using Cysharp.Threading.Tasks;

public class ExitToMenuButton : AbstractMenuExitButton
{
    protected override async UniTask AsyncSetLisnener()
    {
        await base.AsyncSetLisnener();

        _button.onClick.AddListener(() =>
        {
            _backgroundDeniedPanel.SetActive(true);
            _quitButtonPressedChecker.OnExitToMenuButton();
            _quitWarningPanel.SetActive(true);
        });
    }
}