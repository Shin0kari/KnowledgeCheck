using System;
using UnityEngine;
using Zenject;

[Serializable]
public class TreasureChest : MonoBehaviour
{
    public CharacterData character = new();

    public class Factory : PlaceholderFactory<UnityEngine.Object, TreasureChest> { }
}