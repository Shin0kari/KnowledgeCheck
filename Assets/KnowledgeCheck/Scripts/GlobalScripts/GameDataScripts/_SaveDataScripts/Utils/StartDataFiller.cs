using TMPro;
using UnityEngine;
using Zenject;

public class StartDataFiller : IStartDataFiller
{
    private ISaveNameGenerator _saveNameGenerator;

    [Inject]
    private void Construct(ISaveNameGenerator saveNameGenerator)
    {
        _saveNameGenerator = saveNameGenerator;
    }

    public SaveData SetStartData()
    {
        string saveText = "";
        _saveNameGenerator.GenerateSaveName(ref saveText);

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
            CountScore = 0,
            GameTime = 0,
            IsCurrentSave = true,
            Player = player,
            Enemies = enemies
        };
    }
}