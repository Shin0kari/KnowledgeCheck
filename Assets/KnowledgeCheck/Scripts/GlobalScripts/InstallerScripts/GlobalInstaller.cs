using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] private AssetReferenceT<CoreContextSO> _coreContext;

    public override void InstallBindings()
    {
        Debug.Log("Global: Start Install");

        BindProviders(); // GlobalOnEveryScene
        BindCoreContext(); // GlobalOnEveryScene
        BindAdditionalProviders();

        BindGameDataValidator();
        BindFileService();
        BindGameData();
        BindSaveService();
        BindGameDataChanger();
        BindGameStarterService();
        BindAudioService();

        // BindBoostrap();
    }

    private void BindProviders()
    {
        Container.BindInterfacesAndSelfTo<StaticResourceProvider>().FromNew().AsSingle().NonLazy();
        BindAddressablesProviders();
        Container.BindInterfacesAndSelfTo<AssetProvider>().FromNew().AsSingle().WithConcreteId("Global").NonLazy();
    }

    private void BindAddressablesProviders()
    {
        Container.BindInterfacesAndSelfTo<AddressablesDataProvider>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ResourceLocationProvider>().FromNew().AsSingle().NonLazy();
    }

    private void BindCoreContext()
    {
        Container
            .BindInterfacesAndSelfTo<CoreContextProvider>()
            .FromNew()
            .AsCached()
            .WithConcreteId("Global")
            .WithArguments(_coreContext)
            .NonLazy();
    }

    private void BindAdditionalProviders()
    {
        Container.BindInterfacesAndSelfTo<CoreGlobalAudioProvider>().AsSingle().NonLazy();
    }

    private void BindGameDataValidator()
    {
        Container.BindInterfacesAndSelfTo<ValidatorGameData>().AsSingle().NonLazy();
    }

    private void BindFileService()
    {
        Container.BindInterfacesAndSelfTo<FileChecker>().AsSingle().NonLazy();
        Container.Bind<SaveFolderPath>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<FileDataLoader>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<FileDataSaver>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<FileDataDeleter>().AsSingle().NonLazy();
    }

    private void BindGameData()
    {
        Container.BindInterfacesAndSelfTo<GameData>().AsSingle().NonLazy();
    }

    private void BindSaveService()
    {
        BindSaveCreatorService();
        BindSaveDeleterService();
        BindSaveUpdaterService();
    }

    private void BindSaveCreatorService()
    {
        Container.BindInterfacesAndSelfTo<StartDataFiller>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SaveCreator>().AsSingle().NonLazy();
    }

    private void BindSaveDeleterService()
    {
        Container.BindInterfacesAndSelfTo<SaveDeleter>().AsSingle().NonLazy();
    }

    private void BindSaveUpdaterService()
    {
        Container.BindInterfacesAndSelfTo<SaveUpdater>().AsSingle().NonLazy();
    }

    private void BindGameDataChanger()
    {
        Container.Bind<GameDataChanger>().AsSingle().NonLazy();
    }

    private void BindGameStarterService()
    {
        Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LoadingScreenController>().AsSingle().NonLazy();
        Container.Bind<ChoicedSceneLoader>().AsSingle().NonLazy();
        Container.Bind<GameStarter>().AsSingle().NonLazy();
    }

    private void BindAudioService()
    {
        Container.BindInterfacesAndSelfTo<AudioService>().AsSingle().NonLazy();
    }

    // private void BindBoostrap()
    // {
    //     Container.BindInterfacesAndSelfTo<GlobalBoostrap>()
    //     .FromNew()
    //     .AsTransient()
    //     .NonLazy();
    // }
}