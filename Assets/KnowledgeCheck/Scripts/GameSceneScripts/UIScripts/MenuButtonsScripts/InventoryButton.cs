using System;
using Cysharp.Threading.Tasks;
using R3;

public class InventoryButton : AbstractMenuButton
{
    protected override void SubscribeOnUpdateObjects()
    {
        _assetProvider
            .GetIBindingSingletonComponent<InventoryPanelLinker>()
            .OfType<IBindingSingletonComponent, InventoryPanelLinker>()
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
