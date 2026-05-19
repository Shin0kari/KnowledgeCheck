using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "GlobalAudioSO", menuName = "Audio SO/Global Audio SO")]
public class GlobalAudioSO : ScriptableObject
{
    [field: SerializeField]
    public AssetReferenceT<AudioClip> SimpleClickSound
    {
        get;
        private set;
    }
    [field: SerializeField]
    public AssetReferenceT<AudioClip> ClickPanelSound
    {
        get;
        private set;
    }
}