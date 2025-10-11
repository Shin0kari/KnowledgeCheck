using System;
using UnityEngine;
using Zenject;

public class StartMenuSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        BindButtons();
        BindAdditionalScrollPanels();
        BindScrollUpdater();
        BindButtonAvailabilityChecker();
        BindButtonRegistry();
        BindGameCreator();
        BindLoadingScreenView();

        BindDebugScripts();
    }

    private void BindButtons()
    {
        BindMainMenuButtons();
        BindScrollButtons();
    }

    private void BindMainMenuButtons()
    {
        Container.BindInterfacesAndSelfTo<NewGameButton>().FromComponentInHierarchy().AsCached();
        Container.BindInterfacesAndSelfTo<ContinueGameButton>().FromComponentInHierarchy().AsCached();

        Container.Bind<IButton>().WithId("NewGameButton").To<NewGameButton>().FromResolve().AsCached().NonLazy();
        Container.Bind<LoadMenuButton>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<IButton>().WithId("ContinueGameButton").To<ContinueGameButton>().FromResolve().AsCached().NonLazy();
    }

    private void BindScrollButtons()
    {
        Container.BindInterfacesAndSelfTo<NewSaveButton>().FromComponentInHierarchy().AsCached();

        Container.Bind<IButton>().WithId("NewSaveButton").To<NewSaveButton>().FromResolve().AsCached().NonLazy();
    }

    private void BindButtonRegistry()
    {
        Container.Bind<IButtonRegistry>().To<ButtonRegistry>().AsSingle().NonLazy();
    }

    private void BindAdditionalScrollPanels()
    {
        Container.Bind<DeleteSavePanel>().FromComponentInHierarchy().AsSingle().NonLazy();
    }

    private void BindScrollUpdater()
    {
        Container.Bind<IScrollUtils>().To<ScrollUtils>().FromComponentInHierarchy().AsSingle().NonLazy();

        Container.Bind<IUpdateScroll>().To<ScrollUpdateMethod>().AsSingle().NonLazy();
        Container.Bind<IUpdatedObject>().To<UpdatedScrollObject>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SaveChecker>().AsSingle().NonLazy();
    }

    private void BindButtonAvailabilityChecker()
    {
        Container.BindInterfacesAndSelfTo<CheckButtonAvailabilty>().AsSingle().NonLazy();
    }

    private void BindGameCreator()
    {
        Container.Bind<NewGame>().AsSingle().NonLazy();
        Container.Bind<ContinueGame>().AsSingle().NonLazy();
        Container.Bind<NewSave>().AsSingle().NonLazy();
        Container.Bind<LoadSave>().AsSingle().NonLazy();

        Container.Bind<NewGameCreator>().AsSingle().NonLazy();
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