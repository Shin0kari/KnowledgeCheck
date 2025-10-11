using System;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindLoadingScreenView();

        BindDebugScripts();
    }

    private void BindLoadingScreenView()
    {
        Container.BindInterfacesAndSelfTo<LoadingScreenView>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindDebugScripts()
    {
        Container.BindInterfacesAndSelfTo<PrintDataButton>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}