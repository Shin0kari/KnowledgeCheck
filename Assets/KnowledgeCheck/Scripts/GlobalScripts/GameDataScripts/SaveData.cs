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
    public Character Player { get; set; }
    public Enemies Enemies { get; set; }
}

[Serializable]
public record Enemies
{
    public HashSet<Character> EnemyList { get; set; }
}

[Serializable]
public record Character
{
    public Vector3 Pos { get; set; }
    public Quaternion Direction { get; set; }
    public Loot Loot { get; set; }
}

[Serializable]
public record Loot
{
    public Dictionary<int, Item> LootList { get; set; }
}

[Serializable]
public record Item
{
    public string ItemObject { get; set; }
}