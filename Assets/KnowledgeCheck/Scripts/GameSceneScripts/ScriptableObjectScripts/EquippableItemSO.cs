using UnityEngine;

[CreateAssetMenu(fileName = "New Equippable Item", menuName = "Inventory/Equippable Item")]
public class EquippableItemSO : ItemSO
{
    [SerializeField] protected EquipmentSlot _equipSlot;
    [SerializeField, Min(0)] protected int _maxDurability = 100;
    protected bool _isContainer;
    public int Durability { get; set; }

    private void Awake()
    {
        _itemType = ItemType.Equipment;
    }

    public EquipmentSlot GetEquipmentSlot()
    {
        return _equipSlot;
    }

    public override Item CreateRuntimeItem()
    {
        return new EquippableItem
        {
            ItemName = GetItemName(),
            ItemStats = _itemStats,
            ItemAffects = _itemAffects,
            Durability = this.Durability
        };
    }
}

public enum EquipmentSlot
{
    Any,
    Head,
    LeftHandWeapon,
    RightHandWeapon,
    Chest,
    Container,
    NonEquippable
}
