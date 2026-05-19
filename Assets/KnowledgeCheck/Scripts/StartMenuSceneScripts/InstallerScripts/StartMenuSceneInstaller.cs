using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class StartMenuSceneInstaller : MonoInstaller
{
    [SerializeField] private AssetReferenceT<CoreContextSO> _coreContext;
    [SerializeField] private AudioSource _audioSource;

    public override void InstallBindings()
    {
        BindProviders(); // GlobalOnEveryScene
        BindCoreContext(); // GlobalOnEveryScene
        BindAdditionalProviders();

        BindGameObjectLoader();
        BindButtonRegistry(); // NotGlobalOnEverySceneScripts
        // BindButtons();
        // BindAdditionalScrollPanels(); // NotGlobalOnEverySceneScripts
        BindScrollUpdater(); // NotGlobalOnEverySceneScripts
        BindButtonAvailabilityChecker();
        BindGameCreator(); // NotGlobalOnEverySceneScripts
        // BindLoadingScreenView(); // NotGlobalOnEverySceneScripts

        // BindCursorManager();
        BindSartMenuBoostrap();

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
        Container.Bind<LoadMenuSavePanelsProvider>().FromNew().AsSingle().NonLazy();
    }

    private void BindGameObjectLoader()
    {
        Container.Bind<LoadChildAddressablesGameObjects>().FromComponentInHierarchy().AsCached().NonLazy();
    }

    private void BindButtonRegistry()
    {
        Container.BindInterfacesAndSelfTo<ButtonRegistry>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    // private void BindButtons()
    // {
    //     BindMainMenuButtons();
    //     BindScrollButtons(); // NotGlobalOnEverySceneScripts
    // }

    // private void BindMainMenuButtons()
    // {
    //     Container.BindInterfacesAndSelfTo<NewGameButton>().FromComponentInHierarchy().AsSingle();
    //     Container.BindInterfacesAndSelfTo<ContinueGameButton>().FromComponentInHierarchy().AsSingle();

    //     Container.BindInterfacesAndSelfTo<LoadMenuButton>().FromComponentInHierarchy().AsSingle().NonLazy();
    // }

    // private void BindScrollButtons()
    // {
    //     Container.BindInterfacesAndSelfTo<ScrollNewGameButton>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    // }

    // private void BindAdditionalScrollPanels()
    // {
    //     Container.BindInterfacesAndSelfTo<DeleteSavePanel>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    // }

    private void BindScrollUpdater()
    {
        // Container.BindInterfacesAndSelfTo<ScrollUtils>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindFactory<UnityEngine.Object, SavePanel, SavePanel.Factory>().FromFactory<PrefabFactory<SavePanel>>(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<SavePanelFactory>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts

        Container.BindInterfacesAndSelfTo<ScrollUpdateMethod>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<SaveChecker>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<UpdatedScrollObject>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindButtonAvailabilityChecker()
    {
        Container.BindInterfacesAndSelfTo<CheckButtonAvailabilty>().FromNew().AsSingle().NonLazy();
    }

    private void BindGameCreator()
    {
        Container.BindInterfacesAndSelfTo<NewGame>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<ContinueGame>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        // Container.BindInterfacesAndSelfTo<NewSave>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<LoadGameData>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts

        Container.BindInterfacesAndSelfTo<NewGameCreator>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    // private void BindLoadingScreenView() // NotGlobalOnEverySceneScripts
    // {
    //     Container.BindInterfacesAndSelfTo<LoadingScreenView>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    // }

    // private void BindCursorManager()
    // {
    //     Container.BindInterfacesAndSelfTo<CursorManager>().FromNew().AsTransient().NonLazy();
    // }

    private void BindSartMenuBoostrap()
    {
        Container
            .BindInterfacesAndSelfTo<StartMenuBoostrap>()
            .FromNew()
            .AsSingle()
            .WithArguments(_audioSource)
            .NonLazy();
    }

    // private void BindDebugScripts()
    // {
    //     Container.BindInterfacesAndSelfTo<PrintDataButton>().FromComponentInHierarchy().AsSingle().NonLazy();
    // }
}