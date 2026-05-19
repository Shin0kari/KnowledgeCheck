using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScrollUpdateMethod : IUpdateScroll, IDisposable
{
    private IAssetProviderGetter _assetProvider;
    private IGetGameData _gameData;

    private SavePanelFactory _savePanelFactory;
    private HashSet<IScrollUtils> _scrollsUtils = new();

    private List<GameObject> _savePanelsForOneSave = new();

    private DisposableBag _dB;
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        IGetGameData gameData,
        SavePanelFactory savePanelFactory)
    {
        _assetProvider = assetProvider;
        _gameData = gameData;
        _savePanelFactory = savePanelFactory;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        DisposeTokens();
        DisposeBags();
        DisposeDynamicProperty();
    }

    private void DisposeTokens()
    {
        _ct?.Cancel();
        _ct?.Dispose();
    }

    private void DisposeBags()
    {
        _dB.Dispose();
    }

    private void DisposeDynamicProperty()
    {
        _scrollsUtils.Clear();
        _savePanelsForOneSave.Clear();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingTransientComponent<IScrollUtils>()
            .Subscribe((listScrollUtils) =>
            {
                if (listScrollUtils == null || listScrollUtils.Count < 1)
                    return;
                FillScroll(listScrollUtils).Forget();
            })
            .AddTo(ref _dB);
    }

    private async UniTaskVoid FillScroll(List<IBindingTransientComponent> listScrollUtils)
    {
        foreach (IScrollUtils scrollUtils in listScrollUtils.Cast<IScrollUtils>())
        {
            if (!_scrollsUtils.Contains(scrollUtils))
            {
                await UpdateScrollDataToCurrent(scrollUtils);
            }
        }
    }

    private async UniTask UpdateScrollDataToCurrent(IScrollUtils scrollUtils)
    {
        _scrollsUtils.Add(scrollUtils);

        DeleteScrollSaves(scrollUtils);

        foreach (var save in _gameData.GetAllGameDatas())
        {
            var savePanelObject = await _savePanelFactory.InstantiateSave(scrollUtils, _ct.Token);
            if (savePanelObject == null) return;

            var savePanelData = savePanelObject.GetComponent<SavePanel>();
            savePanelData.SetSaveData(save.Value.SaveName, save.Value.Uuid);
        }
    }

    public void CreateAllSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        try
        {
            DisposeTokens();
            _ct = new();

            DeleteSaves();

            foreach (var save in saves)
            {
                AddSave(save.Value, _ct.Token).Forget();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при создании всех панелей с сохранениями: {ex.Message}");
        }
    }

    public async UniTaskVoid AddSave(SaveData saveData, CancellationToken ct)
    {
        _savePanelsForOneSave = new();
        try
        {
            // в InstantiateSavePanels заполняется _savePanelsForOneSave
            await InstantiateSavePanels(ct);

            foreach (var newSavePanel in _savePanelsForOneSave)
            {
                var savePanelData = newSavePanel.GetComponent<SavePanel>();
                savePanelData.SetSaveData(saveData.SaveName, saveData.Uuid);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при создании панели с сохранением: {ex.Message}");
            foreach (var newSavePanel in _savePanelsForOneSave)
            {
                _savePanelFactory.DestroyInstanceOnScroll(newSavePanel);
            }
        }
    }

    public async UniTaskVoid AddSave(SaveData saveData)
    {
        _savePanelsForOneSave = new();
        try
        {
            // в InstantiateSavePanels заполняется _savePanelsForOneSave
            await InstantiateSavePanels(_ct.Token);

            foreach (var newSavePanel in _savePanelsForOneSave)
            {
                var savePanelData = newSavePanel.GetComponent<SavePanel>();
                savePanelData.SetSaveData(saveData.SaveName, saveData.Uuid);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при создании панели с сохранением: {ex.Message}");
            foreach (var newSavePanel in _savePanelsForOneSave)
            {
                _savePanelFactory.DestroyInstanceOnScroll(newSavePanel);
            }
        }
    }

    private async UniTask InstantiateSavePanels(CancellationToken ct)
    {
        foreach (var scrollsUtils in _scrollsUtils)
        {
            var savePanelObject = await _savePanelFactory.InstantiateSave(scrollsUtils, ct);
            _savePanelsForOneSave.Add(savePanelObject);
        }
    }

    public void DeleteMissingSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        foreach (var scrollUtils in _scrollsUtils)
        {
            foreach (var savePanelObject in scrollUtils.GetAllContent())
            {
                if (!saves.ContainsKey(savePanelObject.GetComponent<SavePanel>().GetSaveUuid()))
                    _savePanelFactory.DestroyInstanceOnScroll(savePanelObject);
            }
        }
    }

    public void UpdateCurrentSave((string uuid, SaveData saveData) currentSave)
    {
        try
        {
            foreach (var scrollUtils in _scrollsUtils)
            {
                foreach (var savePanelObject in scrollUtils.GetAllContent())
                {
                    var savePanel = savePanelObject.GetComponent<SavePanel>();

                    if (currentSave.saveData.SaveName == savePanel.GetSaveName())
                    {
                        savePanel.SetSaveData(currentSave.saveData.SaveName, currentSave.saveData.Uuid);
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ошибка при обновлении всех скроллов: {ex.Message}");
        }
    }

    public void UpdateAllSaves(IReadOnlyDictionary<string, SaveData> saves)
    {
        CreateAllSaves(saves);
    }

    private void DeleteSaves()
    {
        foreach (var scrollUtils in _scrollsUtils)
        {
            DeleteScrollSaves(scrollUtils);
        }
    }

    private void DeleteScrollSaves(IScrollUtils scrollUtils)
    {
        if (scrollUtils.GetCountContent() > 1)
        {
            for (int i = 1; i < scrollUtils.GetCountContent(); i++)
            {
                _savePanelFactory.DestroyInstanceOnScroll(scrollUtils.GetScrollChildGameObject(i));
            }
        }

        if (scrollUtils.GetCountContent() < 5)
        {
            scrollUtils.SetActiveStateForNewSaveButton(true);
        }
    }
}
