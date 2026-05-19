using UnityEngine;
using Zenject;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] protected InventoryItem _item;

    public InventoryItem GetInventoryItem()
    {
        return _item;
    }

    public class Factory : PlaceholderFactory<Object, ItemPanel> { }
}