using System;
using TMPro;
using UnityEngine;

public abstract class AbstractSavePanel : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _saveName;
    protected string _uuid;

    public string GetSaveName()
    {
        return _saveName.text;
    }

    public string GetSaveUuid()
    {
        return _uuid;
    }

    public void SetSaveData(string newSaveName, string newSaveUuid)
    {
        _saveName.text = newSaveName;
        _uuid = newSaveUuid;
    }
}
