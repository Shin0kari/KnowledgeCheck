using R3;

public class SaveGamePanelButton : AbstractMenuButton
{
    protected override void SubscribeOnUpdateObjects()
    {
        _assetProvider
            .GetIBindingSingletonComponent<SaveGamePanelLinker>()
            .OfType<IBindingSingletonComponent, SaveGamePanelLinker>()
            .Subscribe(inventoryPanelLinker =>
            {
                if (inventoryPanelLinker == null)
                    return;

                _linkedPanel = inventoryPanelLinker.LinkerObject;
                _linkedPanelLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }
}