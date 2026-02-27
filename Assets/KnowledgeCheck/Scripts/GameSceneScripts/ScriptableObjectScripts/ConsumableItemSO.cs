using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Inventory/Consumable Item")]
public class ConsumableItemSO : ItemSO
{
    [SerializeField, Min(1)] protected int _quantity = 1;

    private void Awake()
    {
        _itemType = ItemType.Consumable;
    }

    public int GetQuantity()
    {
        return _quantity;
    }

    public void ChangeQuantity(int additionalQuantity)
    {
        int newQuantity = _quantity + additionalQuantity;
        SetQuantity(newQuantity);
    }

    public void SetQuantity(int newQuantity)
    {
        if (newQuantity < 1)
            _quantity = 0;
        else if (newQuantity > GetMaxStackQuantity())
            _quantity = GetMaxStackQuantity();
        else
            _quantity = newQuantity;
    }

    public override Item CreateRuntimeItem()
    {
        return new ConsumableItem
        {
            ItemName = GetItemName(),
            ItemStats = _itemStats,
            ItemAffects = _itemAffects,
            Quantity = _quantity
        };
    }
}