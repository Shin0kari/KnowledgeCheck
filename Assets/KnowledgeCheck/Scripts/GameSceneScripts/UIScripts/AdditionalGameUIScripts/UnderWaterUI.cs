using System;
using UnityEngine;
using Zenject;

public class UnderWaterUI : MonoBehaviour
{
    private const float MIN_FADE = 0f;

    [SerializeField] private CanvasGroup _underWaterUI;
    [SerializeField] private float _underWaterCanvasFade = 0.3f;

    private void Awake()
    {
        _underWaterUI.alpha = MIN_FADE;
        _underWaterUI.blocksRaycasts = false;
        _underWaterUI.interactable = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ChangeUnderWaterUIActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ChangeUnderWaterUIActive(false);
        }
    }

    private void ChangeUnderWaterUIActive(bool isUnderWater)
    {
        float newFadeValue;
        if (isUnderWater)
            newFadeValue = _underWaterCanvasFade;
        else
            newFadeValue = MIN_FADE;

        _underWaterUI.alpha = newFadeValue;
    }
}