using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuAmbientAudioSO", menuName = "Audio SO/Main Menu Ambient Audio SO", order = 0)]
public class MainMenuAmbientAudioSO : ScriptableObject
{
    public AudioList AmbientAudio = new();
}