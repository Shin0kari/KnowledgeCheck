using UnityEngine;
using Zenject;

public class ContainerSlotFactory : AbstractFactoryStarter
{
    private GameObject _itemPanelPrefab;
    private ItemPanel.Factory _itemPanelFactory;

    [Inject]
    private void Construct(
        GameObject itemPanelPrefab,
        ItemPanel.Factory itemPanelFactory
    )
    {
        _itemPanelPrefab = itemPanelPrefab;
        _itemPanelFactory = itemPanelFactory;
    }

    public ItemPanel SpawnPanelOnInventory(GameObject inventoryPanel)
    {
        var itemPanel = _itemPanelFactory.Create(_itemPanelPrefab);
        itemPanel.transform.SetParent(inventoryPanel.transform);
        return itemPanel;
    }

    public void DespawnPanelsInInventory(GameObject inventoryPanel)
    {
        for (int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
            Object.Destroy(inventoryPanel.transform.GetChild(i).gameObject);
        }
    }
}