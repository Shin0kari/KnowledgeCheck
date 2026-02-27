using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GateSound : MonoBehaviour
{
    private AudioSource _source;

    [SerializeField] private AudioClip _openGateSound;
    [SerializeField] private AudioClip _closeGateSound;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void PlayOpenGateSound()
    {
        _source.PlayOneShot(_openGateSound);
    }
    public void PlayCloseGateSound()
    {
        _source.PlayOneShot(_closeGateSound);
    }
}