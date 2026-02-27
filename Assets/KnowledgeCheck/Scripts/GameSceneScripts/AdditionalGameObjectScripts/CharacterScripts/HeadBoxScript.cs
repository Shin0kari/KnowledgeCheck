using System;
using UnityEngine;

public class HeadBoxScript : MonoBehaviour
{
    public event Action<bool> ChangeUnderWaterState;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ChangeUnderWaterState?.Invoke(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ChangeUnderWaterState?.Invoke(false);
        }
    }


}