using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using Zenject;

public class ItemPanelRegistry : IDisposable
{
    private PlayableCharacterDataUpdater _characterDataUpdater;
    private readonly List<InventoryItem> _items = new();

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(PlayableCharacterDataUpdater characterDataUpdater)
    {
        _characterDataUpdater = characterDataUpdater;
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();

        _items.Clear();
    }

    public void Register(InventoryItem item)
    {
        _items.Add(item);
        item.OnUpdate += UpdateData;
    }

    public void Unregister(InventoryItem item)
    {
        _items.Remove(item);
        item.OnUpdate -= UpdateData;
    }

    private void UpdateData()
    {
        _characterDataUpdater.UpdateCharacterData().Forget();
    }

    public List<InventoryItem> GetItemPanels() => _items;
}