using System;
using Cysharp.Threading.Tasks;
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
        if (_characterDataUpdater != null)
            _characterDataUpdater.OnDataUpdate -= UpdateContainerInventoryUI;
    }

    public void UpdateContainerInventoryUI()
    {
        _inventoryFiller.FillContainerInventoryFromContainerSO().Forget();
    }
}