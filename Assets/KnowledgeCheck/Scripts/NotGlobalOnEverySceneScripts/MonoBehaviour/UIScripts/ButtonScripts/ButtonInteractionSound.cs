using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ButtonInteractionSound : MonoBehaviour, IPointerDownHandler
{
    private IAudioService _globalAudioService;

    [Inject]
    private void Construct(
        IAudioService globalAudioService)
    {
        _globalAudioService = globalAudioService;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _globalAudioService.OnUIClickButtonPanel();
    }
}