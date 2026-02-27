using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ValidatorGameData : IValidatorGameData
{
    private readonly List<IValidationRule<SaveData>> _saveDataRules;
    private readonly List<IValidationRule<CharacterData>> _characterRules;

    [Inject]
    public ValidatorGameData()
    {
        _saveDataRules = new List<IValidationRule<SaveData>>
        {
            new SaveNameRule(),
            new PlayerExistRule(),
            new ScoreNonNegativeRule()
        };

        _characterRules = new List<IValidationRule<CharacterData>>
        {
            // new CharacterPositionRule(),
            // new CharacterDirectionRule(),
            // new CharacterLootRule()
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
            if (rule == null) Debug.LogError("[VALIDATOR_GAMEDATA]: Rule is null.");
            if (data == null) Debug.Log("[VALIDATOR_GAMEDATA]: Save is null.");
            if (data.SaveName == null) Debug.LogError("[VALIDATOR_GAMEDATA]: SaveName is null.");


            if (!rule.Validate(data, out var error))
            {
                Debug.LogError($"GAME_RULE_ERROR: {error}");
                isValid = false;
            }
        }

        isValid &= ValidateCharacter(data.Player);

        return isValid;
    }

    public bool ValidateCharacter(CharacterData character)
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