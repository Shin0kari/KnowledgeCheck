using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ButtonItemGenerator : MonoBehaviour, IChangeButtonInteractable, IBindingSingletonComponent
{
    [SerializeField] private Button _button;
    public event Action IsUsed;

    private void Awake()
    {
        BindAllTypes();
    }

    private void OnDestroy()
    {
        IsUsed = null;
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            IsUsed?.Invoke();
        });
    }

    public void DisableButton()
    {
        _button.interactable = false;
    }

    public void EnableButton()
    {
        _button.interactable = true;
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}
