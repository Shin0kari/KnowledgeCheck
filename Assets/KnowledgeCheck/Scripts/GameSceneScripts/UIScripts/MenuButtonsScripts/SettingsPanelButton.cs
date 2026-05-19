using R3;

public class SettingsPanelButton : AbstractMenuButton
{
    protected override void SubscribeOnUpdateObjects()
    {
        _assetProvider
            .GetIBindingSingletonComponent<SettingsPanelLinker>()
            .OfType<IBindingSingletonComponent, SettingsPanelLinker>()
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