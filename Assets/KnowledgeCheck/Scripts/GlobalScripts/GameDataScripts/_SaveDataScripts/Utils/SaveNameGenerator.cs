using System;
using TMPro;

public class SaveNameGenerator : ISaveNameGenerator
{
    public void GenerateSaveName(ref string saveName)
    {
        DateTime now = DateTime.Now;

        // saveName = $"Save\nData: {now.Second:D2}:{now.Minute:D2}:{now.Hour:D2} {now.Day:D2}:{now.Month:D2}:{now.Year % 100:D2}";
        saveName = $"Save_Data time_{now.Hour:D2}_{now.Minute:D2}_{now.Second:D2} day_{now.Day:D2}_{now.Month:D2}_{now.Year % 100:D2}";
    }
}