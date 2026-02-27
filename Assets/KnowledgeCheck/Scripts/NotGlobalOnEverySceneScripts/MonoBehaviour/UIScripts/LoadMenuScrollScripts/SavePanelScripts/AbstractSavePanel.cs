using System;
using TMPro;
using UnityEngine;

public abstract class AbstractSavePanel : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _saveName;

    public string GetSaveName()
    {
        return _saveName.text;
    }

    public void SetSaveName(string newSaveName)
    {
        _saveName.text = newSaveName;
    }
}
