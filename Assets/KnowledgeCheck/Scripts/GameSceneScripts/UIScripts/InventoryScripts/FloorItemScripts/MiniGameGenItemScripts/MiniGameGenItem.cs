using System;
using UnityEngine;

public class MiniGameGenItem : MonoBehaviour
{
    public event Action OnCompleteMiniGame;

    // private void OnEnable()
    // {
    //     SetStartMiniGameData();
    // }

    // private void SetStartMiniGameData()
    // {
    //     transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, RotationUtils.MIN_ROTATION);
    // }

    private void Update()
    {
        if (transform.rotation.eulerAngles.z < -(RotationUtils.MAX_ROTATION - RotationUtils.START_ROTATION_VALUE - 1f))
        {
            OnCompleteMiniGame?.Invoke();
            // SetStartMiniGameData();
        }
    }
}
