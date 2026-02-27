using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public abstract class UIButton : MonoBehaviour
{
    [SerializeField] protected Button _button;
    public event Action IsUsed;

    private ButtonRegistry _buttonRegistry;

    [Inject]
    private void Construct(ButtonRegistry buttonRegistry)
    {
        _buttonRegistry = buttonRegistry;
    }

    private void OnEnable()
    {
        _buttonRegistry?.Register(this);
    }

    private void OnDisable()
    {
        _buttonRegistry?.Unregister(this);

        // IsUsed = null;
    }
    // private void OnDestroy()
    // {
    //     _buttonRegistry?.Unregister(this);
    // }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            ActionOnClick();
            IsUsed?.Invoke();
        });
    }

    protected virtual void ActionOnClick() { }
}