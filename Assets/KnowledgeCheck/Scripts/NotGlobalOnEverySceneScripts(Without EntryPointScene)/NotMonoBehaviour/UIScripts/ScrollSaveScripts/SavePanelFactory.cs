using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class SavePanelFactory : IDisposable
{
    private IAssetProviderGetter _assetProvider;

    private SavePanel _savePanel;
    private SavePanel.Factory _savePanelFactory;
    private DeleteSavePanel _deleteSavePanel;

    private DisposableBag _dB;
    private UniTaskCompletionSource _loadedDeleteSavePanel = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        SavePanel.Factory savePanelFactory,
        IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;
        _savePanelFactory = savePanelFactory;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        _loadedDeleteSavePanel?.TrySetCanceled();
        _loadedDeleteSavePanel = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<DeleteSavePanel>()
            .OfType<IBindingSingletonComponent, DeleteSavePanel>()
            .Subscribe(deleteSavePanel =>
            {
                if (deleteSavePanel == null)
                    return;
                _deleteSavePanel = deleteSavePanel;
                _loadedDeleteSavePanel.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    public async UniTask<GameObject> InstantiateSave(IScrollUtils scrollsUtils, CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, ct);

        try
        {
            var savePrefab = await scrollsUtils.GetSavePrefab(linkedCTS.Token);
            if (savePrefab == null)
                return null;

            _savePanel = _savePanelFactory.Create(savePrefab);
            _savePanel.transform.SetParent(scrollsUtils.GetScroll().content);
            _savePanel.transform.localScale = new(1f, 1f, 1f);

            await _loadedDeleteSavePanel.Task.AttachExternalCancellation(linkedCTS.Token);

            _savePanel.GetDeleteSaveButton().SetDeleteSavePanel(_deleteSavePanel);

            return _savePanel.gameObject;
        }
        catch (System.OperationCanceledException)
        {
            return null;
        }

    }

    public void DestroyInstanceOnScroll(GameObject savePanelObject)
    {
        UnityEngine.Object.Destroy(savePanelObject);
    }
}