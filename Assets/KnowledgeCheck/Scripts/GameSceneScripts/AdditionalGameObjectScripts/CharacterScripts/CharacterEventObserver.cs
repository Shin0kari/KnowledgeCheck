using System;
using UnityEngine;

public class CharacterEventObserver : MonoBehaviour, IDisposable
{
    [SerializeField] protected CharacterTimer _characterTimer;
    [SerializeField] protected FallChecker _groundChecker;
    [SerializeField] protected CharacterAnimation _characterAnimation;
    [SerializeField] protected GameObject _heartBoxObject;

    private HeartBox _heartBox;

    public event Action OnDrownState;
    public event Action OnDeathState;
    public event Action OnLandState;
    public event Action OnFallState;
    public event Action OnImpactState;

    public event Action OnDeath;
    public event Action OnSpawn;
    public event Action OnSetDefaultState;
    public event Action OnSetOffControlState;

    protected void Awake()
    {
        _heartBox = _heartBoxObject.GetComponent<HeartBox>();

        _heartBox.OnGetDamage += SetImpactState;
        _characterTimer.CharacterDrowning += SetDrownState;
        _characterTimer.CharacterDrowned += SetFinalDrownState;
        _groundChecker.FallStarted += SetFallState;
        _groundChecker.LandStarted += SetLandState;
    }

    public void Dispose()
    {
        if (_heartBox != null) _heartBox.OnGetDamage -= SetImpactState;
        if (_characterTimer != null)
        {
            _characterTimer.CharacterDrowning -= SetDrownState;
            _characterTimer.CharacterDrowned -= SetFinalDrownState;
        }
        if (_groundChecker != null)
        {
            _groundChecker.FallStarted -= SetFallState;
            _groundChecker.LandStarted -= SetLandState;
        }

        OnDrownState = null;
        OnDeathState = null;
        OnLandState = null;
        OnFallState = null;
        OnImpactState = null;
        OnDeath = null;
        OnSpawn = null;
        OnSetDefaultState = null;
    }

    private void SetImpactState()
    {
        _characterAnimation.SetCharacterImpactAnim();
        OnImpactState?.Invoke();
    }

    public virtual void SetDrownState()
    {
        _characterAnimation.SetCharacterDrownAnim();
    }

    private void SetFinalDrownState()
    {
        _characterAnimation.SetCharacterFinalDrownAnim();
        _heartBoxObject.SetActive(false);
        OnDrownState?.Invoke();

        OnDeath?.Invoke();
    }

    protected void SetFallState()
    {
        _characterAnimation.SetCharacterFallAnim();
        OnFallState?.Invoke();
    }

    protected void SetLandState()
    {
        _characterAnimation.SetCharacterLandAnim();
        OnLandState?.Invoke();
    }

    public virtual void SetDeathState()
    {
        _characterAnimation.SetCharacterDeathAnim();
        _heartBoxObject.SetActive(false);
        OnDeathState?.Invoke();

        OnDeath?.Invoke();
    }

    public virtual void SetSpawnState()
    {
        _heartBoxObject.SetActive(true);
        _characterAnimation.SetCharacterSpawnAnim();
        OnSpawn?.Invoke();
    }

    public void SetIdleState()
    {
        _characterAnimation.SetCharacterIdleAnim();
    }

    public void SetDefaultState()
    {
        OnSetDefaultState?.Invoke();
    }

    public void SetOffControlState()
    {
        OnSetOffControlState?.Invoke();
    }
}