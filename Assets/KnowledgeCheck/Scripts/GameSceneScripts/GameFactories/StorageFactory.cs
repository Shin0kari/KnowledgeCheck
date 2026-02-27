using Zenject;

public class StorageFactory : AbstractFactoryStarter, IInitializable
{
    readonly TreasureChest.Factory _storegeFactory;

    public StorageFactory(TreasureChest.Factory storegeFactory)
    {
        _storegeFactory = storegeFactory;
    }

    // Спавн хранилища
    public void Initialize()
    {
        if (!_isFactoryActive)
        {
            return;
        }
    }
}
