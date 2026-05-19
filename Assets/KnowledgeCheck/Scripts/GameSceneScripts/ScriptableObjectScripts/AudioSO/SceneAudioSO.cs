using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SceneAudioSO", menuName = "Audio SO/Scene Audio SO")]
public class SceneAudioSO : ScriptableObject
{
    [SerializeField] private AssetReferenceT<SceneMusicsSO> _sceneMusicsSO;
    public AssetReferenceT<SceneMusicsSO> SceneMusicsSO => _sceneMusicsSO;

    [SerializeField] private AssetReferenceT<SceneAmbientSoundsSO> _sceneAmbientSoundsSO;
    public AssetReferenceT<SceneAmbientSoundsSO> SceneAmbientSoundsSO => _sceneAmbientSoundsSO;

    [SerializeField] private AssetReferenceT<SceneInteractionsSoundsSO> _sceneInteractionsSoundsSO;
    public AssetReferenceT<SceneInteractionsSoundsSO> SceneInteractionsSoundsSO => _sceneInteractionsSoundsSO;
}