using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ValidatorGameData : IValidatorGameData
{
    private readonly List<IValidationRule<SaveData>> _saveDataRules;
    private readonly List<IValidationRule<Character>> _characterRules;

    [Inject]
    public ValidatorGameData()
    {
        Debug.Log("[VALIDATOR_GAMEDATA]: init data.");
        _saveDataRules = new List<IValidationRule<SaveData>>
        {
            new SaveNameRule(),
            new PlayerExistRule(),
            new ScoreNonNegativeRule()
        };

        _characterRules = new List<IValidationRule<Character>>
        {
            new CharacterPositionRule(),
            new CharacterDirectionRule(),
            new CharacterLootRule()
        };
    }

    public bool ValidateGameData(SaveData data)
    {
        bool isValid = true;

        if (data == null)
        {
            isValid = false;
            return isValid;
        }

        foreach (var rule in _saveDataRules)
        {
            if (rule == null) Debug.Log("[VALIDATOR_GAMEDATA]: Rule is null.");
            if (data == null) Debug.Log("[VALIDATOR_GAMEDATA]: Save is null.");
            if (data.SaveName == null) Debug.Log("[VALIDATOR_GAMEDATA]: SaveName is null.");


            if (!rule.Validate(data, out var error))
            {
                Debug.LogError($"GAME_RULE_ERROR: {error}");
                isValid = false;
            }
        }

        isValid &= ValidateCharacter(data.Player);

        return isValid;
    }

    public bool ValidateCharacter(Character character)
    {
        bool isValid = true;

        foreach (var rule in _characterRules)
        {
            if (!rule.Validate(character, out var error))
            {
                Debug.LogWarning($"CHARACTER_RULE_ERROR: {error}");
                isValid = false;
            }
        }

        return isValid;
    }
}