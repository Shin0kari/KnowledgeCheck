using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using Zenject;

public class MenuController : MonoBehaviour, IChangeStateMenuSender, IBindingSingletonComponent
{
    private MenuUtilsProvider _menuUtilsProvider;

    private ReactiveProperty<ScriptableObject> _reactiveMenuUtilsSO;
    private MenuUtilsSO _menuUtilsSO;

    private PlayerInputSystem _playerInput;
    [SerializeField] private GameObject _mainMenu;

    public event Action<bool> ChangeState;

    private bool _isAvailableToSwitch;

    private DisposableBag _dB;

    private CancellationToken _ct;

    [Inject]
    private void Construct(MenuUtilsProvider menuUtilsProvider)
    {
        _menuUtilsProvider = menuUtilsProvider;

        _ct = gameObject.GetCancellationTokenOnDestroy();
        _playerInput = new PlayerInputSystem();
        _isAvailableToSwitch = true;

        BindAllTypes();

        AsyncLoadResource().Forget();
    }

    private void OnDestroy()
    {
        ChangeState = null;
        _dB.Dispose();
    }

    private async UniTask AsyncLoadResource()
    {
        _reactiveMenuUtilsSO = await _menuUtilsProvider.TryGetDataSO(_ct);

        _reactiveMenuUtilsSO?
            .Subscribe(menuUtilsSO =>
            {
                if (menuUtilsSO == null)
                    return;

                SetSO(menuUtilsSO);
            })
            .AddTo(ref _dB);
    }

    private void SetSO(ScriptableObject menuUtilsSO)
    {
        if (menuUtilsSO is not MenuUtilsSO so)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Loaded invalid SO");
            return;
        }

        UpdateMenuUtils(so);
    }

    private void UpdateMenuUtils(MenuUtilsSO so)
    {
        if (_playerInput == null)
            return;

        if (_menuUtilsSO == null)
        {
            _menuUtilsSO = so;

            _playerInput.Player.Menu.performed += context => ChangeMenuActiveStatus();
            if (_menuUtilsSO.IsStopGameOnMenu)
            {
                _playerInput.Player.Menu.performed += context => ChangeGameTimeScale();
            }
        }
    }

    private void ChangeMenuActiveStatus()
    {
        if (!_isAvailableToSwitch)
            return;

        _mainMenu.SetActive(!_mainMenu.activeSelf);
        SendChangeMenuStatusSignal(_mainMenu.activeSelf);
    }

    private void SendChangeMenuStatusSignal(bool menuState)
    {
        if (menuState)
        {
            CursorVisibility.OnCursorVisibility();
        }
        else
        {
            CursorVisibility.OffCursorVisibility();
        }
        ChangeState?.Invoke(menuState);
    }

    public void ChangeMenuAvailableState(bool newState)
    {
        _isAvailableToSwitch = newState;
    }

    private void ChangeGameTimeScale()
    {
        if (_mainMenu.activeSelf)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        _playerInput?.Enable();
    }

    private void OnDisable()
    {
        _playerInput?.Disable();
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}

public interface IChangeStateMenuSender : IBindingSingletonComponent
{
    public event Action<bool> ChangeState;
}