using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneInteractionsSoundsSO", menuName = "Audio SO/Scene Interactions Sounds SO")]
public class SceneInteractionsSoundsSO : ScriptableObject
{
    [field: SerializeField] private StoneDoorAudio _stoneDoorAudio = new();
    public StoneDoorAudio StoneDoorAudio => _stoneDoorAudio;
}

[Serializable]
public class StoneDoorAudio
{
    [field: SerializeField] private AudioList _openStoneDoorAudio = new();
    public AudioList OpenStoneDoorAudio => _openStoneDoorAudio;

    [field: SerializeField] private AudioList _closeStoneDoorAudio = new();
    public AudioList CloseStoneDoorAudio => _closeStoneDoorAudio;
}