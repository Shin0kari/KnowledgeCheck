using System;
using UnityEngine;
using Zenject;

[Serializable]
public class Car : MonoBehaviour
{
    public CharacterData character = new();

    public class Factory : PlaceholderFactory<UnityEngine.Object, Car> { }
}