using System;
using UnityEngine;

public interface INotPlayableCharacter : IAbstractCharacter
{
    public event Action<Enemy> Spawned;
}
