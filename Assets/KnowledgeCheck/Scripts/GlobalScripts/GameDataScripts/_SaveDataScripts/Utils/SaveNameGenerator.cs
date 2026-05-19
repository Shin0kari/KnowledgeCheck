using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Zenject;

public static class SaveNameGenerator
{
    private static readonly Regex TargetPattern = new(@"_(\d*)?$", RegexOptions.Compiled);

    public static string GenerateSaveName()
    {
        DateTime now = DateTime.Now;

        return $"Save_Data time_{now.Hour:D2}_{now.Minute:D2}_{now.Second:D2} day_{now.Day:D2}_{now.Month:D2}_{now.Year % 100:D2}_";
    }

    public static string GenerateSaveName(string saveName)
    {
        if (string.IsNullOrEmpty(saveName))
        {
            saveName = GenerateSaveName();
            return saveName;
        }

        Match match = TargetPattern.Match(saveName);

        if (match.Success)
        {
            string numberPart = match.Groups[1].Value;

            if (string.IsNullOrEmpty(numberPart))
            {
                saveName += "1";
            }
            else
            {
                int currentNumber = int.Parse(numberPart);
                int nextNumber = currentNumber + 1;

                saveName = saveName[..match.Index] + "_" + nextNumber;
            }
        }
        else
        {
            saveName += "_1";
        }
        return saveName;
    }
}