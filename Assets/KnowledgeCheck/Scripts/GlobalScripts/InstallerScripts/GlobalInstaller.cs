using System;
using UnityEngine;
using Zenject;

public class GlobalInstaller : MonoInstaller
{
    [SerializeField] private AudioSource _startMenuSceneAmbient;
    [SerializeField] private AudioSource _gameSceneAmbient;
    [SerializeField] private AudioSource _additionalAudioSource;
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _clickPanelSound;

    public override void InstallBindings()
    {
        BindGameDataValidator();
        BindFileService();
        BindGameData();
        BindSaveService();
        BindGameDataChanger();
        BindGameStarterService();
        BindAudioService();
    }

    private void BindGameDataValidator()
    {
        Container.Bind<IValidatorGameData>().To<ValidatorGameData>().AsSingle().NonLazy();
    }

    private void BindFileService()
    {
        Container.Bind<IFileChecker>().To<FileChecker>().AsSingle().NonLazy();
        Container.Bind<SaveFolderPath>().AsSingle().NonLazy();
        Container.Bind<ILoadData>().To<FileDataLoader>().AsSingle().NonLazy();
        Container.Bind<ISaveData>().To<FileDataSaver>().AsSingle().NonLazy();
        Container.Bind<IDeleteData>().To<FileDataDeleter>().AsSingle().NonLazy();
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
        Container.Bind<ISaveNameGenerator>().To<SaveNameGenerator>().AsSingle().NonLazy();
        Container.Bind<IStartDataFiller>().To<StartDataFiller>().AsSingle().NonLazy();
        Container.Bind<ISaveCreator>().To<SaveCreator>().AsSingle().NonLazy();
    }

    private void BindSaveDeleterService()
    {
        Container.Bind<ISaveDeleter>().To<SaveDeleter>().AsSingle().NonLazy();
    }

    private void BindSaveUpdaterService()
    {
        Container.Bind<ISaveUpdater>().To<SaveUpdater>().AsSingle().NonLazy();
    }

    private void BindGameDataChanger()
    {
        Container.Bind<GameDataChanger>().AsSingle().NonLazy();
    }

    private void BindGameStarterService()
    {
        Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LoadingScreenController>().AsSingle().NonLazy();
        Container.Bind<ChoicedSceneLoader>().AsSingle().NonLazy();
        Container.Bind<GameStarter>().AsSingle().NonLazy();
    }

    private void BindAudioService()
    {
        Container.Bind<IAudioService>().To<AudioService>()
            .AsSingle()
            .WithArguments(
                _startMenuSceneAmbient,
                _gameSceneAmbient,
                _clickSound,
                _clickPanelSound)
            .NonLazy();
    }
}