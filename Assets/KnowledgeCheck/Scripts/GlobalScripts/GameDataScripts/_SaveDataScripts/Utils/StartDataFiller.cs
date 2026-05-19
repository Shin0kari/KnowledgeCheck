using System;
using TMPro;
using UnityEngine;
using Zenject;

public class StartDataFiller : IStartDataFiller
{

    public string GenerateSaveName()
    {
        string saveText = SaveNameGenerator.GenerateSaveName();
        return saveText;
    }

    public string GenerateSaveName(string saveName)
    {
        string saveText = SaveNameGenerator.GenerateSaveName(saveName);
        return saveText;
    }

    public string GenerateUuid()
    {
        return Guid.NewGuid().ToString();
    }

    public SaveData SetStartData()
    {
        string saveText = GenerateSaveName();

        string uuid = GenerateUuid();

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

        return new SaveData
        {
            SaveName = saveText,
            Uuid = uuid,
            CountScore = 0,
            GameTime = 0,
            IsNewGame = true,
            IsCurrentSave = true,
            Player = player
        };
    }
}