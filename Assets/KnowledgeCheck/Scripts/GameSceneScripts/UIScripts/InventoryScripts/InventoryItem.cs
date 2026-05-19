using System;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Image))]
public class InventoryItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Sprite _emptyItemSprite;
    [SerializeField] private ItemSO _currentItemData = null; // при пустом слоте и должен быть null
    [SerializeField] private ItemType _slotType;
    [SerializeField] private TextMeshProUGUI _quantityItemText;
    [SerializeField] private EquipmentSlot _equipmentSlotTypeValue;

    private EquipmentSlot _equipmentSlotType;
    private InventoryItem _cursor;
    private GameCursorItemManager _cursorManager;
    private ItemPanelRegistry _itemPanelRegistry;
    private InventoryFiller _inventoryFiller;

    public event Action OnUpdate;

    public EquipmentSlot EquipmentSlotType
    {
        get => _equipmentSlotType;
        private set
        {
            if (_slotType == ItemType.Equipment)
                _equipmentSlotType = value;
            else if (_slotType == ItemType.Any)
                _equipmentSlotType = EquipmentSlot.Any;
            else
                _equipmentSlotType = EquipmentSlot.NonEquippable;
        }
    }

    [Inject]
    private void Construct(
        GameCursorItemManager cursorManager,
        ItemPanelRegistry itemPanelRegistry,
        InventoryFiller inventoryFiller)
    {

        _cursorManager = cursorManager;
        _itemPanelRegistry = itemPanelRegistry;
        _inventoryFiller = inventoryFiller;

        TryGetCursorItem();

        InitSlotData();
    }

    private void OnDestroy()
    {
        if (_cursorManager != null)
            _cursorManager.OnCursorLoaded -= SetCursor;

        if (_itemPanelRegistry != null)
            _itemPanelRegistry.Unregister(this);

        ClearAllData();
        OnUpdate = null;
    }

    private void ClearAllData()
    {
        SetCurrentItemData(null);
    }

    private void TryGetCursorItem()
    {
        _cursor = _cursorManager.GetCursorItem();
        if (_cursor == null)
        {
            _cursorManager.OnCursorLoaded += SetCursor;
        }
    }

    private void SetCursor(InventoryItem cursorItem)
    {
        _cursor = cursorItem;
    }

    private void InitSlotData()
    {
        EquipmentSlotType = _equipmentSlotTypeValue;
        ClearAllData();

        _itemPanelRegistry.Register(this);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (_cursor == null)
            return;

        var cursorItemSO = _cursor.GetCurrentItemData();
        if (_currentItemData == null && cursorItemSO == null)
            return;

        bool isOperationComplete = false;
        switch (_slotType)
        {
            case ItemType.Equipment:
                isOperationComplete = StepsForEquippableSlot(cursorItemSO);
                break;
            case ItemType.Consumable:
                isOperationComplete = StepsForConsumableSlot(cursorItemSO);
                break;
            default:    // default - ItemType.Any
                isOperationComplete = StepsForAnySlot(cursorItemSO);
                _inventoryFiller.FillContainerSOFromContainerInventory().Forget();
                break;
        }

        if (isOperationComplete)
            UpdateCharacterStats();
    }

    private bool StepsForEquippableSlot(ItemSO cursorItemSO)
    {
        if (TryExchangeIfItemNull(cursorItemSO))
            return true;

        switch (cursorItemSO.GetItemType())
        {
            case ItemType.Equipment:
                if ((cursorItemSO as EquippableItemSO).GetEquipmentSlot() != EquipmentSlotType)
                    return false;

                ItemDataExchange();
                return true;

            default:
                return false;
        }
    }

    private bool StepsForConsumableSlot(ItemSO cursorItemSO)
    {
        if (TryExchangeIfItemNull(cursorItemSO))
            return true;

        switch (cursorItemSO.GetItemType())
        {
            case ItemType.Consumable:
                if (TryExchangeIfItemNull(_currentItemData))
                    return true;

                return StepsForConsumableCursorItem(cursorItemSO);

            default:
                return false;
        }
    }

    private bool StepsForAnySlot(ItemSO cursorItemSO)
    {
        if (TryExchangeIfItemNull(cursorItemSO))
            return true;

        if (TryExchangeIfItemNull(_currentItemData))
            return true;

        if (_currentItemData.GetItemType() != cursorItemSO.GetItemType())
        {
            ItemDataExchange();
            return true;
        }

        if (_currentItemData.GetItemType() == ItemType.Equipment)
        {
            ItemDataExchange();
            return true;
        }

        return StepsForConsumableCursorItem(cursorItemSO);
    }

    private bool StepsForConsumableCursorItem(ItemSO cursorItemSO)
    {
        if (_currentItemData.GetItemName() != cursorItemSO.GetItemName())
        {
            ItemDataExchange();
            return true;
        }

        var cursorConsumableItemSO = cursorItemSO as ConsumableItemSO;
        var currentConsumableItemSO = _currentItemData as ConsumableItemSO;

        if (cursorConsumableItemSO.GetQuantity() == cursorConsumableItemSO.GetMaxStackQuantity() ||
            currentConsumableItemSO.GetQuantity() == currentConsumableItemSO.GetMaxStackQuantity())
        {
            ItemDataExchange();
            return true;
        }

        var quantityCurrentItemSO = currentConsumableItemSO.GetQuantity();
        var quantityCursorItemSO = cursorConsumableItemSO.GetQuantity();

        currentConsumableItemSO.ChangeQuantity(quantityCursorItemSO);
        cursorConsumableItemSO.ChangeQuantity(-(cursorConsumableItemSO.GetMaxStackQuantity() - quantityCurrentItemSO));

        CheckZeroQuantity();
        _cursor.CheckZeroQuantity();

        return true;
    }

    private bool TryExchangeIfItemNull(ItemSO itemSO)
    {
        if (itemSO == null)
        {
            ItemDataExchange();
            return true;
        }
        return false;
    }

    private void UpdateCharacterStats()
    {
        OnUpdate?.Invoke();
    }

    private void ItemDataExchange()
    {
        var cloneCursorItemData = TryReturnCloneItemData(_cursor.GetCurrentItemData());

        _cursor.SetCurrentItemData(TryReturnCloneItemData(_currentItemData));

        SetCurrentItemData(cloneCursorItemData);
    }

    public void SetCurrentItemData(ItemSO itemData)
    {
        _currentItemData = itemData;
        UpdateItem();
    }

    public static T TryReturnCloneItemData<T>(T itemData) where T : ItemSO
    {
        T newItemData = null;
        if (itemData != null)
        {
            newItemData = Instantiate(itemData);
            newItemData.name = itemData.name;
        }

        return newItemData;
    }

    public void UpdateItem()
    {
        UpdateItemSprite();
        UpdateItemQuantity();
    }

    private void UpdateItemQuantity()
    {
        if (_currentItemData is ConsumableItemSO)
        {
            _quantityItemText.gameObject.SetActive(true);
            _quantityItemText.text = (_currentItemData as ConsumableItemSO).GetQuantity().ToString();
        }
        else
            _quantityItemText.gameObject.SetActive(false);
    }

    private void CheckZeroQuantity()
    {
        if (_currentItemData is ConsumableItemSO)
        {
            if ((_currentItemData as ConsumableItemSO).GetQuantity() < 1)
            {
                SetCurrentItemData(null);
                return;
            }
        }
        UpdateItem();
    }

    private void UpdateItemSprite()
    {
        Sprite choicedImege;
        if (_currentItemData == null)
        {
            choicedImege = _emptyItemSprite;
        }
        else
        {
            choicedImege = _currentItemData.GetIcon();
        }

        GetComponent<Image>().sprite = choicedImege;
    }

    public ItemSO GetCurrentItemData()
    {
        return _currentItemData;
    }
}