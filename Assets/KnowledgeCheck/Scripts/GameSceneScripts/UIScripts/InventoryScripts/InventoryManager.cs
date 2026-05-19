using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IBindingSingletonComponent
{
    [SerializeField] private InventoryItem _headItem;
    [SerializeField] private InventoryItem _chestItem;
    [SerializeField] private InventoryItem _leftHandItem;
    [SerializeField] private InventoryItem _rightHandItem;

    [SerializeField] private InventoryItem _container;

    [SerializeField] private GameObject _parentContainerInventoryObject;

    public readonly ReactiveProperty<bool> InventoryState = new();

    private void OnDestroy()
    {
        InventoryState.Dispose();
    }

    private void Awake()
    {
        BindAllTypes();
    }

    private void OnEnable()
    {
        InventoryState.Value = true;
        // OnEnableInventory?.Invoke();
    }

    private void OnDisable()
    {
        InventoryState.Value = false;
        // OnDisableInventory?.Invoke();
    }

    public MainInventoryItems GetMainItems()
    {
        return new MainInventoryItems()
        {
            headItem = _headItem,
            chestItem = _chestItem,
            leftHandItem = _leftHandItem,
            rightHandItem = _rightHandItem
        };
    }

    public AdditionalInventoryItems GetAdditionalItems()
    {
        return new AdditionalInventoryItems()
        {
            container = _container
        };
    }

    public List<InventoryItem> GetContainerInventoryItems()
    {
        List<InventoryItem> inventoryItems = new();
        if (_parentContainerInventoryObject.transform.childCount == 0)
            return inventoryItems;

        foreach (var item in _parentContainerInventoryObject.GetComponentsInChildren<InventoryItem>())
        {
            inventoryItems.Add(item);
        }
        return inventoryItems;
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}

public struct MainInventoryItems
{
    public InventoryItem headItem;
    public InventoryItem chestItem;
    public InventoryItem leftHandItem;
    public InventoryItem rightHandItem;
}

public struct AdditionalInventoryItems
{
    public InventoryItem container;
}