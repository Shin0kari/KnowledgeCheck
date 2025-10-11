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
        Debug.Log("[STARTDATA_FILLER]: savename: " + saveText);

        Character player = new()
        {
            Pos = new Vector3(),
            Direction = Quaternion.identity,
            Loot = new Loot()
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