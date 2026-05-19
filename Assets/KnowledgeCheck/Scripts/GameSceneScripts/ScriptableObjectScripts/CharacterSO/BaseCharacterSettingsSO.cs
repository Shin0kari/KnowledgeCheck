using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class BaseCharacterSettingsSO : ScriptableObject
{
    public AssetReferenceT<GameObject> CharacterPrefab { get { return _characterPrefab; } }
    [SerializeField] private AssetReferenceT<GameObject> _characterPrefab;

    public string CharacterName { get { return _characterName; } }
    [SerializeField] private string _characterName;

    public CharacterType CharacterType { get { return _characterType; } }
    [SerializeField] private CharacterType _characterType;

    public CharacterData CharacterBaseData { get { return _characterBaseData; } }
    [SerializeField] private CharacterData _characterBaseData;
}

public enum CharacterType
{
    CommonFriend,
    EliteFriend,
    CommonEnemy,
    EliteEnemy,
    Boss,
    Player
}