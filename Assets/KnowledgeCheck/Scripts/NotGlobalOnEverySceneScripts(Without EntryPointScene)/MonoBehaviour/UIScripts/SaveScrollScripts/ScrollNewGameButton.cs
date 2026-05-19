using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class ScrollNewGameButton : UIButton, IChangeButtonVisible, IBindingSingletonComponent
{
    private ScrollNewGameButton()
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