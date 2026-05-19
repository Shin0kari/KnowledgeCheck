using R3;

public class LoadGamePanelButton : AbstractMenuButton
{
    protected override void SubscribeOnUpdateObjects()
    {
        _assetProvider
            .GetIBindingSingletonComponent<LoadGamePanelLinker>()
            .OfType<IBindingSingletonComponent, LoadGamePanelLinker>()
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