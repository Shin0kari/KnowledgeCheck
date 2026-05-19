using System;
using UnityEngine;
using Zenject;

public class EntryPointSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindProviders(); // GlobalOnEveryScene
        // BindCoreContext(); // GlobalOnEveryScene
        BindStartSceneChanger();
        BindStartSceneBoostrap();
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

    // private void BindCoreContext()
    // {
    //     Container.Bind<CoreContextSpawner>().AsSingle().NonLazy();
    // }

    private void BindStartSceneChanger()
    {
        Container.BindInterfacesAndSelfTo<ChangeScene>().FromNew().AsSingle().NonLazy();
    }

    private void BindStartSceneBoostrap()
    {
        Container.BindInterfacesAndSelfTo<EntryPointSceneBoostrap>().FromNew().AsSingle().NonLazy();
    }
}