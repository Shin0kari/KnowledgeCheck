using System;
using UnityEngine;

public class StarterArenaEvent : MonoBehaviour
{
    public event Action<Player> PlayerOnStarter;
    public event Action<Player> PlayerLeftStarter;

    void OnTriggerEnter(Collider enteringGameObject)
    {
        if (enteringGameObject.CompareTag("Player"))
        {
            enteringGameObject.GetComponent<ButtonArenaStateToggle>().enabled = true;

            PlayerOnStarter?.Invoke(enteringGameObject.GetComponent<Player>());
        }
    }

    void OnTriggerExit(Collider leavingGameObject)
    {
        if (leavingGameObject.CompareTag("Player"))
        {
            leavingGameObject.GetComponent<ButtonArenaStateToggle>().enabled = false;

            PlayerLeftStarter?.Invoke(leavingGameObject.GetComponent<Player>());
        }
    }
}
