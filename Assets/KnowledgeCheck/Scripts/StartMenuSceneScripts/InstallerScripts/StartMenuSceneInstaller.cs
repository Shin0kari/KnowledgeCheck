using System;
using UnityEngine;
using Zenject;

public class StartMenuSceneInstaller : MonoInstaller
{
    [SerializeField] private AudioSource _audioSource;

    public override void InstallBindings()
    {
        BindButtons();
        BindAdditionalScrollPanels(); // NotGlobalOnEverySceneScripts
        BindScrollUpdater(); // NotGlobalOnEverySceneScripts
        BindButtonAvailabilityChecker();
        BindButtonRegistry(); // NotGlobalOnEverySceneScripts
        BindGameCreator(); // NotGlobalOnEverySceneScripts
        BindLoadingScreenView(); // NotGlobalOnEverySceneScripts

        // BindCursorManager();
        BindSartMenuBoostrap();

        BindDebugScripts();
    }

    private void BindButtons()
    {
        BindMainMenuButtons();
        BindScrollButtons(); // NotGlobalOnEverySceneScripts
    }

    private void BindMainMenuButtons()
    {
        Container.BindInterfacesAndSelfTo<NewGameButton>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<ContinueGameButton>().FromComponentInHierarchy().AsSingle();

        // Container.Bind<IButton>().WithId("NewGameButton").To<NewGameButton>().FromResolve().AsCached().NonLazy();
        Container.BindInterfacesAndSelfTo<LoadMenuButton>().FromComponentInHierarchy().AsSingle().NonLazy();
        // Container.Bind<IButton>().WithId("ContinueGameButton").To<ContinueGameButton>().FromResolve().AsCached().NonLazy();
    }

    private void BindScrollButtons()
    {
        Container.BindInterfacesAndSelfTo<ScrollNewGameButton>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts

        // Container.Bind<IButton>().WithId("ScrollNewGameButton").To<ScrollNewGameButton>().FromResolve().AsCached().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindButtonRegistry()
    {
        Container.BindInterfacesAndSelfTo<ButtonRegistry>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindAdditionalScrollPanels()
    {
        Container.BindInterfacesAndSelfTo<DeleteSavePanel>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindScrollUpdater()
    {
        Container.BindInterfacesAndSelfTo<ScrollUtils>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindFactory<UnityEngine.Object, SavePanel, SavePanel.Factory>().FromFactory<PrefabFactory<SavePanel>>(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<SavePanelFactory>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts

        Container.BindInterfacesAndSelfTo<ScrollUpdateMethod>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<UpdatedScrollObject>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<SaveChecker>().FromNew().AsSingle().NonLazy();
    }

    private void BindButtonAvailabilityChecker()
    {
        Container.BindInterfacesAndSelfTo<CheckButtonAvailabilty>().FromNew().AsSingle().NonLazy();
    }

    private void BindGameCreator()
    {
        Container.BindInterfacesAndSelfTo<NewGame>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<ContinueGame>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<NewSave>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
        Container.BindInterfacesAndSelfTo<LoadGameData>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts

        Container.BindInterfacesAndSelfTo<NewGameCreator>().FromNew().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    private void BindLoadingScreenView() // NotGlobalOnEverySceneScripts
    {
        Container.BindInterfacesAndSelfTo<LoadingScreenView>().FromComponentInHierarchy().AsSingle().NonLazy(); // NotGlobalOnEverySceneScripts
    }

    // private void BindCursorManager()
    // {
    //     Container.BindInterfacesAndSelfTo<CursorManager>().FromNew().AsTransient().NonLazy();
    // }

    private void BindSartMenuBoostrap()
    {
        Container.BindInterfacesAndSelfTo<StartMenuBoostrap>().FromNew().AsSingle().WithArguments(_audioSource).NonLazy();
    }

    private void BindDebugScripts()
    {
        Container.BindInterfacesAndSelfTo<PrintDataButton>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}