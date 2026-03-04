using System.Linq;
using UnityEngine;

public class SaveNameRule : IValidationRule<SaveData>, IValidationRule<string>
{
    private const int MAX_SAVENAME_LENGHT = 100;
    public bool Validate(SaveData data, out string errorMessage) => Validate(data.SaveName, out errorMessage);

    public bool Validate(string saveName, out string errorMessage)
    {
        if (string.IsNullOrEmpty(saveName))
        {
            errorMessage = "Save name is empty.";
            return false;
        }
        if (saveName.Length >= MAX_SAVENAME_LENGHT)
        {
            errorMessage = "Save name is too long (max 100).";
            return false;
        }
        if (saveName.Any(c => char.IsControl(c) || c == '\\' || c == '/' || c == '|'))
        {
            errorMessage = "Save name contains invalid characters.";
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