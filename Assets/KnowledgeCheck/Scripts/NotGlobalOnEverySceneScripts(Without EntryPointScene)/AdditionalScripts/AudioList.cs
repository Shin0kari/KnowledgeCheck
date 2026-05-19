using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class AudioList
{
    [field: SerializeField] private List<AssetReferenceT<AudioClip>> _clips;

    public List<AssetReferenceT<AudioClip>> GetClipsReference()
    {
        return _clips;
    }
}