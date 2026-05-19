using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private AssetReferenceT<CoreContextSO> _coreContext;
    [SerializeField] private AudioSource _audioSource;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        BindProviders(); // GlobalOnEveryScene
        BindCoreContext(); // GlobalOnEveryScene
        BindAdditionalProviders();
        BindScriptableObjectRepositories();

        BindGameObjectLoader();

        BindCamera();

        BindActorInteractionSounds();
        BindSceneCharacterDataFiller();
        BindAnimationUtils();

        BindMenu();

        BindArena();

        BindEnemy();
        BindPlayer();
        // BindPlayerCar();
        // BindTreasure();

        BindHealthBar();
        BindCharactersObserver();

        // BindLoadingScreenView(); // NotGlobalOnEverySceneScripts

        // BindUIItemsDB();
        BindSecondPanel();
        BindAdditionalUIPanel();
        BindGameCreator(); // NotGlobalOnEverySceneScripts

        BindBoostrap();

        // BindDebugScripts();
    }

    private void BindProviders()
    {
        BindAddressablesProviders();
        Container.BindInterfacesAndSelfTo<AssetProvider>().FromNew().AsSingle().NonLazy();
    }

    private void BindAddressablesProviders()
    {
        Container.BindInterfacesAndSelfTo<AddressablesDataProvider>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ResourceLocationProvider>().FromNew().AsSingle().NonLazy();
    }

    private void BindCoreContext()
    {
        Container
            .Bind<CoreContextProvider>()
            .FromNew()
            .AsSingle()
            .WithArguments(_coreContext)
            .NonLazy();
    }

    private void BindAdditionalProviders()
    {
        Container.Bind<ActorInteractionAudioProvider>().FromNew().AsSingle().NonLazy();
        Container.Bind<BaseUIItemInfoProvider>().FromNew().AsSingle().NonLazy();
        Container.Bind<MenuUtilsProvider>().FromNew().AsSingle().NonLazy();
        Container.Bind<SceneAudioProvider>().FromNew().AsSingle().NonLazy();
        Container.Bind<SceneCharactersSettingsProvider>().FromNew().AsSingle().NonLazy();
        Container.Bind<UIUtilsProvider>().FromNew().AsSingle().NonLazy();

        Container.Bind<LoadMenuSavePanelsProvider>().FromNew().AsSingle().NonLazy();
        Container.Bind<SaveMenuSavePanelsProvider>().FromNew().AsSingle().NonLazy();
    }

    private void BindScriptableObjectRepositories()
    {
        Container.Bind<ActorInteractionAudioRepository>().FromNew().AsSingle().NonLazy();
        Container.Bind<SceneCharactersSettingsRepository>().FromNew().AsSingle().NonLazy();
    }

    private void BindGameObjectLoader()
    {
        Container.Bind<LoadChildAddressablesGameObjects>().FromComponentInHierarchy().AsCached().NonLazy();
    }

    private void BindActorInteractionSounds()
    {
        Container.BindInterfacesAndSelfTo<ActorInteractionAudio>().FromNew().AsSingle().NonLazy();
    }

    private void BindSceneCharacterDataFiller()
    {
        Container.BindInterfacesAndSelfTo<SceneCharacterDataFiller>().FromNew().AsSingle().NonLazy();
    }

    private void BindAnimationUtils()
    {
        Container.BindInterfacesAndSelfTo<CurveAnimationUtils>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AnimationUtils>().FromNew().AsSingle().NonLazy();
    }

    private void BindMenu()
    {
        // Container.BindInterfacesAndSelfTo<MenuController>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<MenuStateSwitcher>().FromNew().AsSingle().NonLazy();

        // BindAdditionalPanel(); // NotGlobalOnEverySceneScripts
    }

    // private void BindAdditionalPanel()
    // {
    //     Container.BindInterfacesAndSelfTo<DeleteSavePanel>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    // }

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
        Container.BindInterfacesAndSelfTo<ArenaTimer>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<ArenaController>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<OffAllExternalArenaObjects>().FromNew().AsSingle();
    }

    private void BindEnemy()
    {
        // Для каждого вида врагов создаётся новая пул фабрика
        Container.BindMemoryPool<Enemy, Enemy.Pool>().FromFactory<AddressableEnemyFactory>();
        Container.BindInterfacesAndSelfTo<AddressableEnemyFactory>().AsCached().NonLazy();
        Container.BindInterfacesAndSelfTo<EnemyPoolFiller>().FromNew().AsSingle().NonLazy();
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
        Container.BindInterfacesAndSelfTo<PlayerFactory>().FromNew().AsSingle().NonLazy();
    }

    // private void BindPlayerCar()
    // {
    //     Container.BindFactory<UnityEngine.Object, Car, Car.Factory>().FromFactory<PrefabFactory<Car>>();
    //     Container.BindInterfacesAndSelfTo<CarFactory>().FromNew().AsSingle().WithArguments(_playerCarPrefab).NonLazy();
    // }

    // private void BindTreasure()
    // {
    //     Container.BindFactory<UnityEngine.Object, TreasureChest, TreasureChest.Factory>().FromFactory<PrefabFactory<TreasureChest>>();
    //     Container.BindInterfacesAndSelfTo<StorageFactory>().FromNew().AsSingle().NonLazy();
    // }

    private void BindHealthBar()
    {
        Container.BindFactory<UnityEngine.Object, HealthBar, HealthBar.Factory>().FromFactory<PrefabFactory<HealthBar>>();
        Container.BindInterfacesAndSelfTo<HealthBarFactory>().FromNew().AsSingle().NonLazy();
    }

    private void BindCharactersObserver()
    {
        Container.BindInterfacesAndSelfTo<CharactersObserver>().FromNew().AsSingle().NonLazy();
    }

    // private void BindLoadingScreenView()
    // {
    //     Container.BindInterfacesAndSelfTo<LoadingScreenView>().FromComponentInHierarchy().AsSingle().NonLazy();
    // }

    // private void BindUIItemsDB()
    // {
    //     Container.BindInterfacesAndSelfTo<ItemsDB>().FromComponentInHierarchy().AsSingle().NonLazy();
    // }

    private void BindSecondPanel()
    {
        BindLinkerManager();
        BindInventoryPanel();
        BindSaveLoadGamePanel();
    }

    private void BindLinkerManager()
    {
        Container.BindInterfacesAndSelfTo<SecondPanelUILinkerManager>().FromNew().AsSingle().NonLazy();
    }

    private void BindInventoryPanel()
    {
        // Container.BindInterfacesAndSelfTo<InventoryManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameCursorItemManager>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<FloorItemManager>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayableCharacterDataUpdater>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ContainerChecker>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ItemPanelRegistry>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<UpdateSaveFromInventory>().FromNew().AsSingle().NonLazy();

        Container.BindFactory<UnityEngine.Object, ItemPanel, ItemPanel.Factory>().FromFactory<PrefabFactory<ItemPanel>>();
        Container.BindInterfacesAndSelfTo<ContainerSlotFactory>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InventoryFiller>().FromNew().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<ItemFactory>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<FloorItemSpawner>().FromNew().AsSingle().NonLazy();
    }

    private void BindSaveLoadGamePanel()
    {
        BindButtonRegistry(); // NotGlobalOnEverySceneScripts
        BindScrollButtons();
        BindScrollUpdater(); // NotGlobalOnEverySceneScripts
    }

    private void BindButtonRegistry()
    {
        Container.BindInterfacesAndSelfTo<ButtonRegistry>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindScrollButtons()
    {
        // Container.BindInterfacesAndSelfTo<ScrollNewSaveButton>().FromComponentInHierarchy().AsSingle().NonLazy();

        // Container.BindInterfacesAndSelfTo<ScrollNewGameButton>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindScrollUpdater()
    {
        // Container.BindInterfacesAndSelfTo<ScrollUtils>().FromComponentsInHierarchy().AsCached().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindFactory<UnityEngine.Object, SavePanel, SavePanel.Factory>().FromFactory<PrefabFactory<SavePanel>>(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<SavePanelFactory>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts

        Container.BindInterfacesAndSelfTo<ScrollUpdateMethod>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<SaveChecker>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<UpdatedScrollObject>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindAdditionalUIPanel()
    {
        // Container.BindInterfacesAndSelfTo<WinUIUtils>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<WinUI>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LoseUI>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AdditionalUIController>().FromNew().AsSingle().NonLazy();
    }

    private void BindGameCreator()
    {
        Container.BindInterfacesAndSelfTo<NewGame>().FromNew().AsSingle().NonLazy();  // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<ContinueGame>().FromNew().AsSingle().NonLazy();  // NotGlobalOnEverySceneScripts
        // Container.BindInterfacesAndSelfTo<NewSave>().FromNew().AsSingle().NonLazy();  // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<LoadGameData>().FromNew().AsSingle().NonLazy();  // NotGlobalOnEverySceneScripts

        Container.BindInterfacesAndSelfTo<NewGameCreator>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindBoostrap()
    {
        Container.BindInterfacesAndSelfTo<GameBoostrap>()
        .FromNew()
        .AsTransient()
        .WithArguments(_audioSource)
        .NonLazy();
    }

    // private void BindDebugScripts()
    // {
    //     Container.BindInterfacesAndSelfTo<PrintDataButton>().FromComponentInHierarchy().AsSingle().NonLazy();
    // }
}