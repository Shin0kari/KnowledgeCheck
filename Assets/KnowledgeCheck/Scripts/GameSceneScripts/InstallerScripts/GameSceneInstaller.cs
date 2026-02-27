using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject _playerCarPrefab;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _enemyPrefab;

    [SerializeField] private GameObject _enemyCanvasPrefab;
    [SerializeField] private GameObject _playerCanvasPrefab;

    [SerializeField] private GameObject _parentContainerInventoryPanel;
    [SerializeField] private InventoryItem _cursorItem;
    [SerializeField] private InventoryItem _floorItem;
    [SerializeField] private GameObject _itemPanelPrefab;
    [SerializeField] private InventoryItem[] _floorItems;

    [SerializeField] private CanvasGroup _whiteWindow;
    [SerializeField] private CanvasGroup _winPanelCanvas;
    [SerializeField] private Ease _whiteWindowAnimCurve = Ease.InOutQuart;

    [SerializeField] private float _maxColorCanvasFade = 0.5f;
    [SerializeField] private float _colorCanvasFadeDuration = 0.5f;
    [SerializeField] private float _loseTextFadeDuration = 0.5f;
    [SerializeField] private CanvasGroup _redLoseCanvas;
    [SerializeField] private CanvasGroup _blueLoseCanvas;
    [SerializeField] private CanvasGroup _losePanelCanvas;

    [SerializeField] private AudioSource _menuAudioSource;

    [SerializeField] private AudioClip _stoneStepSound;
    [SerializeField] private AudioClip _metalStepSound;
    [SerializeField] private AudioClip _axeSwingSound;
    [SerializeField] private AudioClip _getHitBoneSound;
    [SerializeField] private AudioClip _getHitPlateArmourSound;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        BindCamera();

        BindMaterialSounds();
        BindSceneCharacterDataFiller();
        BindAnimationUtils();

        BindMenu();

        BindArena();
        BindEnemy();

        BindPlayer();
        BindPlayerCar();
        BindTreasure();

        BindHealthBar();
        BindCharacterObserver();

        BindLoadingScreenView(); // NotGlobalOnEverySceneScripts

        BindUIItemsDB();
        BindInventoryScripts();
        BindAdditionalUIPanel();
        BindGameCreator(); // NotGlobalOnEverySceneScripts

        BindGameBoostrap();

        BindDebugScripts();
    }

    private void BindMaterialSounds()
    {
        Container
            .BindInterfacesAndSelfTo<MaterialSounds>()
            .FromNew()
            .AsSingle()
            .WithArguments(
                _stoneStepSound,
                _metalStepSound,
                _axeSwingSound,
                _getHitBoneSound,
                _getHitPlateArmourSound
            )
            .NonLazy();
    }

    private void BindSceneCharacterDataFiller()
    {
        Container.BindInterfacesAndSelfTo<SceneCharacterDataFiller>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindAnimationUtils()
    {
        Container.BindInterfacesAndSelfTo<CurveAnimationUtils>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindMenu()
    {
        Container.BindInterfacesAndSelfTo<MainMenu>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MenuStateSwitcher>().FromNew().AsSingle().NonLazy();

        BindAdditionalPanel(); // NotGlobalOnEverySceneScripts
    }

    private void BindCamera()
    {
        BindCameraController();
        BindCamerasUtils();
        BindFreeLookCameraController();
    }

    private void BindCameraController()
    {
        Container.BindInterfacesAndSelfTo<CameraTrigger>().FromComponentInHierarchy().AsSingle();
    }

    private void BindCamerasUtils()
    {
        Container.BindInterfacesAndSelfTo<CameraUtils>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindFreeLookCameraController()
    {
        Container.BindInterfacesAndSelfTo<FreeLookCameraPosController>().FromNew().AsSingle().NonLazy();
    }

    private void BindArena()
    {
        Container.BindInterfacesAndSelfTo<ArenaUtils>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ArenaGateController>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ArenaTimer>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<ArenaController>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<OffAllExternalArenaObjects>().FromNew().AsSingle();
    }

    private void BindEnemy()
    {
        Container.BindMemoryPool<Enemy, Enemy.Pool>().WithInitialSize(6).FromComponentInNewPrefab(_enemyPrefab);
        Container.BindInterfacesAndSelfTo<EnemyPoolFactory>().FromNew().AsSingle().NonLazy();
    }


    private void BindPlayer()
    {
        BindPlayerSpawnedSignal();
        BindPlayerFactory();
    }

    private void BindPlayerSpawnedSignal()
    {
        Container.DeclareSignal<PlayerSpawnedSignal>();
    }

    private void BindPlayerFactory()
    {
        Container.BindInterfacesAndSelfTo<PlayerControl>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ViewScriptUtils>().FromNew().AsSingle().NonLazy();
        Container.BindFactory<UnityEngine.Object, Player, Player.Factory>().FromFactory<PrefabFactory<Player>>();
        Container.BindInterfacesAndSelfTo<PlayerFactory>().FromNew().AsSingle().WithArguments(_playerPrefab).NonLazy();
    }

    private void BindPlayerCar()
    {
        Container.BindFactory<UnityEngine.Object, Car, Car.Factory>().FromFactory<PrefabFactory<Car>>();
        Container.BindInterfacesAndSelfTo<CarFactory>().FromNew().AsSingle().WithArguments(_playerCarPrefab).NonLazy();
    }

    private void BindTreasure()
    {
        Container.BindFactory<UnityEngine.Object, TreasureChest, TreasureChest.Factory>().FromFactory<PrefabFactory<TreasureChest>>();
        Container.BindInterfacesAndSelfTo<StorageFactory>().FromNew().AsSingle().NonLazy();
    }

    private void BindHealthBar()
    {
        Container.BindFactory<UnityEngine.Object, HealthBar, HealthBar.Factory>().FromFactory<PrefabFactory<HealthBar>>();
        Container.BindInterfacesAndSelfTo<HealthBarFactory>().FromNew().AsSingle().WithArguments(_enemyCanvasPrefab, _playerCanvasPrefab).NonLazy();
    }

    private void BindCharacterObserver()
    {
        Container.BindInterfacesAndSelfTo<CharactersObserver>().FromNew().AsSingle().NonLazy();
    }

    private void BindLoadingScreenView()
    {
        Container.BindInterfacesAndSelfTo<LoadingScreenView>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindUIItemsDB()
    {
        Container.BindInterfacesAndSelfTo<ItemsDB>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindInventoryScripts()
    {
        BindSecondPanel();
    }

    private void BindSecondPanel()
    {
        BindInventoryPanel();
        BindSaveLoadGamePanel();
    }

    private void BindInventoryPanel()
    {
        Container.BindInterfacesAndSelfTo<InventoryFiller>().FromNew().AsSingle().WithArguments(_parentContainerInventoryPanel).NonLazy();

        Container.BindInterfacesAndSelfTo<InventoryManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameCursorItemManager>().FromNew().AsSingle().WithArguments(_cursorItem).NonLazy();
        Container.BindInterfacesAndSelfTo<FloorItemManager>().FromNew().AsSingle().WithArguments(_floorItem).NonLazy();
        Container.BindInterfacesAndSelfTo<PlayableCharacterDataUpdater>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ContainerChecker>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ItemPanelRegistry>().FromNew().AsSingle().NonLazy();

        Container.BindFactory<UnityEngine.Object, ItemPanel, ItemPanel.Factory>().FromFactory<PrefabFactory<ItemPanel>>();
        Container.BindInterfacesAndSelfTo<ContainerSlotFactory>().FromNew().AsSingle().WithArguments(_itemPanelPrefab).NonLazy();
        Container.BindInterfacesAndSelfTo<ItemFactory>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<FloorItemSpawner>().FromNew().AsSingle().WithArguments(_floorItems).NonLazy();
    }

    private void BindSaveLoadGamePanel()
    {
        BindScrollButtons();
        BindScrollUpdater(); // NotGlobalOnEverySceneScripts
        BindButtonRegistry(); // NotGlobalOnEverySceneScripts
    }

    private void BindScrollButtons()
    {
        Container.BindInterfacesAndSelfTo<ScrollNewSaveButton>().FromComponentInHierarchy().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<ScrollNewGameButton>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindScrollUpdater()
    {
        Container.BindInterfacesAndSelfTo<ScrollUtils>().FromComponentsInHierarchy().AsCached().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindFactory<UnityEngine.Object, SavePanel, SavePanel.Factory>().FromFactory<PrefabFactory<SavePanel>>(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<SavePanelFactory>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts

        Container.BindInterfacesAndSelfTo<ScrollUpdateMethod>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<UpdatedScrollObject>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<SaveChecker>().FromNew().AsSingle().NonLazy();
    }

    private void BindAdditionalPanel()
    {
        Container.BindInterfacesAndSelfTo<DeleteSavePanel>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindButtonRegistry()
    {
        Container.BindInterfacesAndSelfTo<ButtonRegistry>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindAdditionalUIPanel()
    {
        Container.BindInterfacesAndSelfTo<WinUIUtils>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<WinUI>()
            .FromNew()
            .AsSingle()
            .WithArguments(
                _whiteWindow,
                _winPanelCanvas,
                _whiteWindowAnimCurve
            )
            .NonLazy();
        Container.BindInterfacesAndSelfTo<LoseUI>()
            .FromNew()
            .AsSingle()
            .WithArguments(
                _maxColorCanvasFade,
                _colorCanvasFadeDuration,
                _loseTextFadeDuration,
                _redLoseCanvas,
                _blueLoseCanvas,
                _losePanelCanvas
            )
            .NonLazy();
        Container.BindInterfacesAndSelfTo<AdditionalUIController>().FromNew().AsSingle().NonLazy();
    }

    private void BindGameCreator()
    {
        Container.BindInterfacesAndSelfTo<NewGame>().FromNew().AsSingle().NonLazy();  // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<ContinueGame>().FromNew().AsSingle().NonLazy();  // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<NewSave>().FromNew().AsSingle().NonLazy();  // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<LoadGameData>().FromNew().AsSingle().NonLazy();  // NotGlobalOnEverySceneScripts

        Container.BindInterfacesAndSelfTo<NewGameCreator>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindGameBoostrap()
    {
        Container.BindInterfacesAndSelfTo<GameBoostrap>().FromNew().AsTransient().WithArguments(_menuAudioSource).NonLazy();
    }

    private void BindDebugScripts()
    {
        Container.BindInterfacesAndSelfTo<PrintDataButton>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}