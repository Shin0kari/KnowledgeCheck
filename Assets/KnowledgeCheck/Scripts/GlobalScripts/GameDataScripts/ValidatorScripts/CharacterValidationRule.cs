using UnityEngine;

public class CharacterPositionRule : IValidationRule<Character>
{
    public bool Validate(Character character, out string error)
    {
        if (float.IsNaN(character.Pos.x) || float.IsInfinity(character.Pos.x)
            || float.IsNaN(character.Pos.y) || float.IsInfinity(character.Pos.y)
            || float.IsNaN(character.Pos.z) || float.IsInfinity(character.Pos.z)
        )
        {
            error = "Character position contains invalid values.";
            return false;
        }

        error = null;
        return true;
    }
}

public class CharacterDirectionRule : IValidationRule<Character>
{
    public bool Validate(Character character, out string error)
    {
        if (float.IsNaN(character.Direction.x) || float.IsInfinity(character.Direction.x)
            || float.IsNaN(character.Direction.y) || float.IsInfinity(character.Direction.y)
            || float.IsNaN(character.Direction.z) || float.IsInfinity(character.Direction.z)
        )
        {
            error = "Character direction is invalid.";
            return false;
        }

        error = null;
        return true;
    }
}

public class CharacterLootRule : IValidationRule<Character>
{
    public bool Validate(Character character, out string error)
    {
        if (character.Loot == null)
        {
            error = "Character loot data is missing.";
            return false;
        }

        error = null;
        return true;
    }
}