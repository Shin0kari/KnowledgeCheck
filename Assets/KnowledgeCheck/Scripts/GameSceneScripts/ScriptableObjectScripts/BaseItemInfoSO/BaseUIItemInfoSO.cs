using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "BaseUIItemInfoSO", menuName = "Scene Context/UI Info/Base UI Item Info SO", order = 0)]
public class BaseUIItemInfoSO : ScriptableObject
{
    public AssetReferenceT<GameObject> ItemPanelPrefab { get { return _itemPanelPrefabName; } }
    [SerializeField] private AssetReferenceT<GameObject> _itemPanelPrefabName;

    public AssetReferenceT<GameObject> ItemPrefab { get { return _itemPrefabName; } }
    [SerializeField] private AssetReferenceT<GameObject> _itemPrefabName;
}