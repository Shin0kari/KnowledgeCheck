using System.Collections.Generic;
using Zenject;

public class ItemPanelRegistry
{
    private PlayableCharacterDataUpdater _characterDataUpdater;
    private readonly List<InventoryItem> _items = new();

    [Inject]
    private void Construct(PlayableCharacterDataUpdater characterDataUpdater)
    {
        _characterDataUpdater = characterDataUpdater;
    }

    public void Register(InventoryItem item)
    {
        _items.Add(item);
        item.OnUpdate += _characterDataUpdater.UpdateCharacterData;
    }

    public void Unregister(InventoryItem item)
    {
        _items.Remove(item);
        item.OnUpdate -= _characterDataUpdater.UpdateCharacterData;
    }

    public List<InventoryItem> GetItemPanels() => _items;
}