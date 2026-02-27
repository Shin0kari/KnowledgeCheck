using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowToCursor : MonoBehaviour
{
    [SerializeField] private RectTransform _parentTransform;
    [SerializeField] private InventoryManager _inventoryManager;

    private bool isFollowEnabled = false;

    private void Awake()
    {
        _inventoryManager.OnEnableInventory += EnableFollow;
        _inventoryManager.OnDisableInventory += DisableFollow;
    }

    private void OnDestroy()
    {
        _inventoryManager.OnEnableInventory -= EnableFollow;
        _inventoryManager.OnDisableInventory -= DisableFollow;
    }

    private void Update()
    {
        if (isFollowEnabled)
        {
            transform.position = Mouse.current.position.ReadValue();
        }
    }

    private void EnableFollow()
    {
        isFollowEnabled = true;
    }

    private void DisableFollow()
    {
        isFollowEnabled = false;
    }
}