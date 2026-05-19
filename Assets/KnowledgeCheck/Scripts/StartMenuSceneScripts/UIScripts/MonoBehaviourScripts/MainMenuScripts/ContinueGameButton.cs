using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class ContinueGameButton : UIButton, IChangeButtonVisible, IBindingSingletonComponent
{
    private ContinueGameButton()
    {
        BindAllTypes();
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
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
