using UnityEngine;

public class SaveNameRule : IValidationRule<SaveData>
{
    public bool Validate(SaveData data, out string errorMessage)
    {
        if (string.IsNullOrEmpty(data.SaveName))
        {
            errorMessage = "Save name is empty.";
            return false;
        }
        errorMessage = null;
        return true;
    }
}

public class PlayerExistRule : IValidationRule<SaveData>
{
    public bool Validate(SaveData data, out string errorMessage)
    {
        if (data.Player == null)
        {
            errorMessage = $"Save '{data.SaveName}' has no player data.";
            return false;
        }
        errorMessage = null;
        return true;
    }
}

public class ScoreNonNegativeRule : IValidationRule<SaveData>
{
    public bool Validate(SaveData data, out string errorMessage)
    {
        if (data.CountScore < 0)
        {
            errorMessage = $"Save '{data.SaveName}' has negative score.";
            return false;
        }
        errorMessage = null;
        return true;
    }
}