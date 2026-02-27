using System;
using Zenject;

public class ContainerChecker : IDisposable
{
    private PlayableCharacterDataUpdater _characterDataUpdater;
    private InventoryFiller _inventoryFiller;

    [Inject]
    private void Construct(
        PlayableCharacterDataUpdater characterDataUpdater,
        InventoryFiller inventoryFiller
    )
    {
        _characterDataUpdater = characterDataUpdater;
        _inventoryFiller = inventoryFiller;

        _characterDataUpdater.OnDataUpdate += UpdateContainerInventoryUI;
    }

    public void Dispose()
    {
        _characterDataUpdater.OnDataUpdate -= UpdateContainerInventoryUI;
    }

    public void UpdateContainerInventoryUI()
    {
        _inventoryFiller.FillContainerInventoryFromContainerSO();
    }
}