using System;
using TMPro;
using UnityEngine;
using Zenject;

public class StartDataFiller : IStartDataFiller
{

    public string GenerateSaveName()
    {
        string saveText = "";
        SaveNameGenerator.GenerateSaveName(ref saveText);
        return saveText;
    }

    public SaveData SetStartData()
    {
        string saveText = GenerateSaveName();

        string uuid = Guid.NewGuid().ToString();

        MainItems equippableMainItems = new()
        {
            HeadItem = null,
            ChestItem = null,
            LeftHandItem = null,
            RightHandItem = null
        };

        AdditionalItems equippableAdditionalItems = new()
        {
            Container = null
        };

        Inventory inventory = new()
        {
            EquippableMainItems = equippableMainItems,
            EquippableAdditionalItems = equippableAdditionalItems
        };

        CharacterStats characterStats = new()
        {
            Health = 100f,
            Damage = 10f,
            Defense = 0f,
        };

        CharacterAffects characterAffects = new()
        {
            Speed = 1f,
            Regeneration = 0f
        };

        CharacterData player = new()
        {
            Pos = null,
            Direction = Quaternion.identity,
            Inventory = inventory,
            Stats = characterStats,
            Affects = characterAffects
        };

        Enemies enemies = new();

        return new SaveData
        {
            SaveName = saveText,
            Uuid = uuid,
            CountScore = 0,
            GameTime = 0,
            IsCurrentSave = true,
            Player = player,
            Enemies = enemies
        };
    }
}