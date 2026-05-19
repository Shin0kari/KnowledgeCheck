public interface IStartDataFiller
{
    public SaveData SetStartData();
    public string GenerateSaveName();
    public string GenerateSaveName(string saveName);
    public string GenerateUuid();
}
