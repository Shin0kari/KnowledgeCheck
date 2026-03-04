using System;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerEventObserver))]
[RequireComponent(typeof(ButtonArenaStateToggle))]
public class Player : MonoBehaviour, IDamagable, IDisposable
{
    private const float LOWER_HEALTH_VALUE_RANGE = 0f;
    [SerializeField] private float _maxHealthValue = 100f;

    [SerializeField] private CharacterData _character = new();
    [SerializeField] private bool _isImmortal;

    private IGetGameData _gameData;
    private SceneCharacterDataFiller _characterDataFiller;
    private InventoryFiller _inventoryFiller;

    private PlayerEventObserver _characterEventObserver;

    public event Action<float> HealthChanged;
    // public event Action<IDamagable> OnDeath;
    // public event Action OnSpawn;

    [Inject]
    private void Construct(
        IGetGameData gameData,
        SceneCharacterDataFiller characterDataFiller,
        InventoryFiller inventoryFiller)
    {
        _gameData = gameData;
        _characterDataFiller = characterDataFiller;
        _inventoryFiller = inventoryFiller;

        _characterEventObserver = GetComponent<PlayerEventObserver>();

        _gameData.CurrentSaveUpdated += SetCharacterDataFromCurrentSave;
    }

    public void Dispose()
    {
        if (_gameData != null)
        {
            _gameData.CurrentSaveUpdated -= SetCharacterDataFromCurrentSave;
        }
    }

    private void Start()
    {
        SetCharacterDataFromCurrentSave();
        UpdatePlayerData();

        SetPlayerData();

        _characterEventObserver.SetSpawnState();
    }

    private void SetCharacterDataFromCurrentSave()
    {
        var (uuid, saveData) = _gameData.GetCurrentGameData();
        if (uuid == null)
            return;

        _character = _gameData.GetCurrentGameData().saveData.Player;
    }

    private void SetPlayerData()
    {
        var (newPos, newRotation) = _characterDataFiller.FillPlayerPositionAndRotation(_character.Pos, _character.Direction);
        gameObject.transform.SetPositionAndRotation(newPos, newRotation);
        // UpdatePlayerData();
    }

    private void UpdatePlayerData()
    {
        FillInventoryUI();
        SetPlayerHealth();
    }

    private void SetPlayerHealth()
    {
        UpdateHealth();
    }

    private void FillInventoryUI()
    {
        var playerInventory = _character.Inventory;

        _inventoryFiller.FillMainItems(playerInventory.EquippableMainItems);
        _inventoryFiller.FillAdditionalItems(playerInventory.EquippableAdditionalItems);
        _inventoryFiller.FillContainerInventoryFromContainerSO();
    }

    public float GetHealth()
    {
        return _character.Stats.Health;
    }

    public void ChangeHealth(float value)
    {
        if (_isImmortal)
            return;

        _character.Stats.Health -= value;

        if (_character.Stats.Health > _maxHealthValue)
            _character.Stats.Health = _maxHealthValue;
        if (_character.Stats.Health <= LOWER_HEALTH_VALUE_RANGE)
        {
            _character.Stats.Health = LOWER_HEALTH_VALUE_RANGE;

            _characterEventObserver.SetDeathState();
        }

        UpdateHealth();
    }

    private void UpdateHealth()
    {
        HealthChanged?.Invoke(_character.Stats.Health);
    }

    public class Factory : PlaceholderFactory<UnityEngine.Object, Player> { }
}
