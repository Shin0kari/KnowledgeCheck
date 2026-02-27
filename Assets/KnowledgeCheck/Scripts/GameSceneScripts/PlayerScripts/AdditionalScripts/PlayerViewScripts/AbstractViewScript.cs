using UnityEngine;

public abstract class AbstractViewScript : MonoBehaviour, IMove, ILook
{
    public bool Enabled { get; set; } = true;

    public abstract void Look();
    public abstract void Move();
}

public interface IMove
{
    public void Move();
}

public interface ILook
{
    public void Look();
}

public enum StrafeDir
{
    left = -1,
    idle = 0,
    right = 1
}

public enum StraightDir
{
    backward = -1,
    idle = 0,
    forward = 1
}