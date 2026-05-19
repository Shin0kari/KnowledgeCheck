using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 0)]
[Serializable]
public class ItemSO : ScriptableObject
{
    [SerializeField] protected string _itemName;
    [SerializeField] protected ItemType _itemType;

    [SerializeField] protected Sprite _icon;
    [SerializeField] protected string _description;

    [SerializeField, Min(1)] protected int _maxStackQuantity;

    public List<ItemStatModifier> ItemStats = new();
    public List<ItemAffect> ItemAffects = new();

    protected bool _isStackable => _maxStackQuantity > 1;

    public Sprite GetIcon()
    {
        return _icon;
    }

    public ItemType GetItemType()
    {
        return _itemType;
    }

    public string GetItemName()
    {
        return _itemName;
    }

    public int GetMaxStackQuantity()
    {
        return _maxStackQuantity;
    }

    public virtual Item CreateRuntimeItem()
    {
        return new Item
        {
            ItemName = GetItemName(),
            ItemStats = this.ItemStats,
            ItemAffects = this.ItemAffects
        };
    }
}

[Serializable]
public enum ItemType
{
    Any,
    Consumable,
    Equipment,
    Resource,
    Quest
}

[Serializable]
public class ItemStatModifier
{
    [SerializeField] private StatType statType;
    [SerializeField] private float value;
    [SerializeField] private string _description;
}

[Serializable]
public enum StatType
{
    Health,
    Damage,
    Defense
}

[Serializable]
public class ItemAffect
{
    [SerializeField] private AffectType statType;
    [SerializeField] private float value;
    [SerializeField] private string _description;
}

[Serializable]
public enum AffectType
{
    Speed,
    Regeneration
}