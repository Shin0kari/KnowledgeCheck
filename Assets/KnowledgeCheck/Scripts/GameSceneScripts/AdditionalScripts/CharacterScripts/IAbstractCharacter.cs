using System;
using UnityEngine;

public interface IAbstractCharacter
{
    public float GetHealth();
    public void ChangeHealth(float value);
}