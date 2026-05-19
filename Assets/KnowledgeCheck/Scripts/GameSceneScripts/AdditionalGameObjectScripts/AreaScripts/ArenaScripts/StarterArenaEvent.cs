using System;
using UnityEngine;

public class StarterArenaEvent : MonoBehaviour, IBindingSingletonComponent
{
    public event Action<Player> PlayerOnStarter;
    public event Action<Player> PlayerLeftStarter;

    private void Awake()
    {
        BindAllTypes();
    }

    private void OnDestroy()
    {
        PlayerOnStarter = null;
        PlayerLeftStarter = null;
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }

    void OnTriggerEnter(Collider enteringGameObject)
    {
        if (enteringGameObject.TryGetComponent<ButtonArenaStateToggle>(out var button))
        {
            button.enabled = true;

            PlayerOnStarter?.Invoke(enteringGameObject.GetComponent<Player>());
        }
    }

    void OnTriggerExit(Collider leavingGameObject)
    {
        if (leavingGameObject.TryGetComponent<Player>(out var player))
        {
            player.GetComponent<ButtonArenaStateToggle>().enabled = false;

            PlayerLeftStarter?.Invoke(player);
        }
    }
}
