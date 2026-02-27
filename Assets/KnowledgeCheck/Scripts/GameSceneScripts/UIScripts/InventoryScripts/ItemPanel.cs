using UnityEngine;
using Zenject;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] protected InventoryItem _item;

    public class Factory : PlaceholderFactory<UnityEngine.Object, ItemPanel> { }
}