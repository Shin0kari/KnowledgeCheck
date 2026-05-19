using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
[RequireComponent(typeof(PlayerEventObserver))]
[RequireComponent(typeof(ButtonArenaStateToggle))]

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IDamagable
{
    private const float LOWER_HEALTH_VALUE_RANGE = 0f;
    [SerializeField] private float _maxHealthValue = 100f;
    [SerializeField] private bool _isImmortal;

    private CharacterData _character = new();
    private CharacterType _characterType;
    private string _characterName;

    private IGetGameData _gameData;
    private SceneCharacterDataFiller _characterDataFiller;
    private InventoryFiller _inventoryFiller;
    private SaveData _currentSaveData;

    private Rigidbody _rb;
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

        gameObject.SetActive(false);

        _rb = GetComponent<Rigidbody>();
        _characterEventObserver = GetComponent<PlayerEventObserver>();
        _currentSaveData = _gameData.GetCurrentGameData().saveData;

        _gameData.CurrentSaveUpdated += SetCharacterDataFromCurrentSave;
    }

    private void OnDestroy()
    {
        if (_gameData != null)
            _gameData.CurrentSaveUpdated -= SetCharacterDataFromCurrentSave;

        ClearActions();
    }

    public void ClearActions()
    {
        HealthChanged = null;
    }

    private void Start()
    {
        AsyncStart().Forget();
    }

    public PlayerEventObserver GetCharacterEventObserver()
    {
        return _characterEventObserver;
    }

    private async UniTaskVoid AsyncStart()
    {
        FreezeCharacter();

        SetCharacterDataFromCurrentSave();
        AsyncUpdatePlayerData();

        await AsyncSetPlayerData();

        _characterEventObserver.SetSpawnState();
        UnFreezeCharacter();
    }

    private void FreezeCharacter()
    {
        gameObject.SetActive(false);
        _rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void UnFreezeCharacter()
    {
        gameObject.SetActive(true);
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void SetCharacterDataFromCurrentSave()
    {
        var (uuid, saveData) = _gameData.GetCurrentGameData();
        if (uuid == null)
            return;

        if (_currentSaveData.IsNewGame) return;

        _character = saveData.Player;
    }

    private async UniTask AsyncSetPlayerData()
    {
        (Vector3 newPos, Quaternion newRotation) = (new(), new());
        if (_currentSaveData == null || _currentSaveData.Player.Pos == null)
            (newPos, newRotation) =
                await _characterDataFiller.FillPlayerPositionAndRotation(
                    _character.Pos,
                    _character.Direction,
                    this.GetCancellationTokenOnDestroy());
        else
            (newPos, newRotation) = _currentSaveData.IsNewGame ?
                await _characterDataFiller.FillPlayerPositionAndRotation(
                    _character.Pos,
                    _character.Direction,
                    this.GetCancellationTokenOnDestroy()) :
                (_currentSaveData.Player.Pos.Value, _currentSaveData.Player.Direction);
        gameObject.transform.SetPositionAndRotation(newPos, newRotation);
    }

    private void AsyncUpdatePlayerData()
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

        _inventoryFiller.FillMainItems(playerInventory.EquippableMainItems).Forget();
        _inventoryFiller.FillAdditionalItems(playerInventory.EquippableAdditionalItems).Forget();
        _inventoryFiller.FillContainerInventoryFromContainerSO().Forget();
    }

    public float GetHealth()
    {
        return _character.Stats.Health;
    }

    public void ChangeHealth(in float value)
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

    public CharacterType GetCharacterType()
    {
        return _characterType;
    }

    public string GetCharacterName()
    {
        return _characterName;
    }

    public void SetCharacterData(
        CharacterStats characterStats,
        CharacterAffects characterAffects,
        Inventory characterInventory,
        CharacterType characterType,
        string characterName)
    {
        _character = new()
        {
            Pos = transform.position,
            Direction = transform.rotation,
            Inventory = characterInventory,
            Stats = characterStats,
            Affects = characterAffects
        };
        _characterType = characterType;
        _characterName = characterName;
    }

    public class Factory : PlaceholderFactory<UnityEngine.Object, Player> { }
}
