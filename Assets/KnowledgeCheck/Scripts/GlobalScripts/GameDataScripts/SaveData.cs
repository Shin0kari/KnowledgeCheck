using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public record SaveData
{
    public string SaveName { get; set; }
    public int CountScore { get; set; }
    public int GameTime { get; set; }
    public bool IsCurrentSave { get; set; } = false;
    public CharacterData Player { get; set; }
    public Enemies Enemies { get; set; }
}

[Serializable]
public record Enemies
{
    public HashSet<CharacterData> EnemyList { get; set; }
}

[Serializable]
public class CharacterData
{
    [field: SerializeField] public Vector3? Pos { get; set; }
    [field: SerializeField] public Quaternion Direction { get; set; }

    [field: SerializeField] public Inventory Inventory { get; set; }
    [field: SerializeField] public CharacterStats Stats { get; set; }
    [field: SerializeField] public CharacterAffects Affects { get; set; }
}

// [Serializable]
// public class OldCharacterData
// {
//     [field: SerializeField] public Vector3 Pos { get; set; }
//     [field: SerializeField] public Quaternion Direction { get; set; }

//     [field: SerializeField]
//     public float Health
//     {
//         get;
//         set;
//     }

//     // public Loot Loot { get; set; }
// }

[Serializable]
public class CharacterAffects
{
    [field: SerializeField]
    public float Speed
    {
        get;
        set;
    }
    [field: SerializeField]
    public float Regeneration
    {
        get;
        set;
    }
}

[Serializable]
public class CharacterStats
{
    [field: SerializeField]
    public float Health
    {
        get;
        set;
    }

    [field: SerializeField]
    public float Defense
    {
        get;
        set;
    }
    [field: SerializeField]
    public float Damage
    {
        get;
        set;
    }
}

[Serializable]
public record Inventory
{
    [field: SerializeField] public MainItems EquippableMainItems { get; set; }
    [field: SerializeField] public AdditionalItems EquippableAdditionalItems { get; set; }
}

[Serializable]
public record MainItems
{
    [field: SerializeField] public EquippableItem HeadItem { get; set; }
    [field: SerializeField] public EquippableItem ChestItem { get; set; }
    [field: SerializeField] public EquippableItem LeftHandItem { get; set; }
    [field: SerializeField] public EquippableItem RightHandItem { get; set; }
}

[Serializable]
public record AdditionalItems
{
    [field: SerializeField] public ContainerItem Container { get; set; }
}

// [Serializable]
// public record Loot
// {
//     [field: SerializeField] public Dictionary<int, Item> LootList { get; set; }
// }



[Serializable]
public record Item
{
    [field: SerializeField] public string ItemName { get; set; }
    [field: SerializeField] public List<ItemStatModifier> ItemStats { get; set; }
    [field: SerializeField] public List<ItemAffect> ItemAffects { get; set; }
}

[Serializable]
public record EquippableItem : Item
{
    [field: SerializeField] public int Durability { get; set; }
}

[Serializable]
public record ContainerItem : EquippableItem
{
    [field: SerializeField]
    public Dictionary<int, Item> ContainerItems = new();
}

[Serializable]
public record ConsumableItem : Item
{
    public int Quantity { get; set; }
}