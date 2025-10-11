using System.Collections.Generic;
using UnityEngine;

public interface ILoadData
{
    public Dictionary<string, SaveData> LoadData();
}