using UnityEngine;
using Zenject;

public class ItemFactory
{
    private InventoryFiller _inventoryFiller;

    [Inject]
    private void Construct(InventoryFiller inventoryFiller)
    {
        _inventoryFiller = inventoryFiller;
    }

    public void SpawnItemOnPanel(InventoryItem inventoryItem)
    {
        int randomItemIndex = Random.Range(0, _inventoryFiller.AllItemsSO.Count);

        ItemSO rndItem = null;
        TryReturnRndItemSOFromAllItemSO(ref randomItemIndex, ref rndItem);

        _inventoryFiller.TryFillChoicedInventoryItemFromItemSO(inventoryItem, rndItem);
    }

    private void TryReturnRndItemSOFromAllItemSO(ref int randomItemIndex, ref ItemSO rndItem)
    {
        foreach ((string itemName, ItemSO itemSO) in _inventoryFiller.AllItemsSO)
        {
            if (randomItemIndex == 0)
                rndItem = InventoryItem.TryReturnCloneItemData(itemSO);
            randomItemIndex -= 1;
        }
    }
}