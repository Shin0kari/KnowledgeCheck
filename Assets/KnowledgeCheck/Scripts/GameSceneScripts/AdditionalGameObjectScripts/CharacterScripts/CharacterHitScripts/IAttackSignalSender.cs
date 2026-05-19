using System;

public interface IAttackSignalSender
{
    public event Action OnStartAttack;
    public event Action OnEndAttack;

    public void ClearActions();
}