using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

public class UnderWaterUI : MonoBehaviour
{
    private const float MIN_FADE = 0f;

    [SerializeField] private float _underWaterCanvasFade = 0.3f;

    private IAssetProviderGetter _assetProvider;

    private CanvasGroup _underWaterUI;

    private DisposableBag _dB;

    private UniTaskCompletionSource _uiUtilsLoadedSource = new();
    private UniTaskCompletionSource _underWaterCanvasGroupLoadedSource = new();

    private UniTaskCompletionSource _allCanvasGroupLoadedSource = new();

    private CancellationToken _ct;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        _ct = gameObject.GetCancellationTokenOnDestroy();

        SubscribeOnUpdateObjects();
        UploadAddressablesCanvas().Forget();
    }

    private void OnDestroy()
    {
        _uiUtilsLoadedSource.TrySetCanceled();
        _uiUtilsLoadedSource = null;

        _underWaterCanvasGroupLoadedSource.TrySetCanceled();
        _underWaterCanvasGroupLoadedSource = null;

        _allCanvasGroupLoadedSource.TrySetCanceled();
        _allCanvasGroupLoadedSource = null;

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<BluePanelLinker>()
            .OfType<IBindingSingletonComponent, BluePanelLinker>()
            .Subscribe(bluePanelLinker =>
            {
                if (bluePanelLinker == null)
                    return;

                _underWaterUI = bluePanelLinker.LinkerObject.GetComponent<CanvasGroup>();
                _underWaterCanvasGroupLoadedSource.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    private async UniTaskVoid UploadAddressablesCanvas()
    {
        try
        {
            await _uiUtilsLoadedSource.Task.AttachExternalCancellation(_ct);
            await _underWaterCanvasGroupLoadedSource.Task.AttachExternalCancellation(_ct);

            ConfigurePanels();

            _allCanvasGroupLoadedSource.TrySetResult();
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
        catch (System.Exception err)
        {
            ErrorMessageGenerator.GenerateSimpleError(this, err);
        }
    }

    private void ConfigurePanels()
    {
        if (_underWaterUI != null)
        {
            _underWaterUI.alpha = MIN_FADE;
            _underWaterUI.blocksRaycasts = false;
            _underWaterUI.interactable = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ChangeUnderWaterUIActive(true).Forget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            ChangeUnderWaterUIActive(false).Forget();
        }
    }

    private async UniTask ChangeUnderWaterUIActive(bool isUnderWater)
    {
        await _allCanvasGroupLoadedSource.Task.AttachExternalCancellation(_ct);
        float newFadeValue;
        if (isUnderWater)
            newFadeValue = _underWaterCanvasFade;
        else
            newFadeValue = MIN_FADE;

        _underWaterUI.alpha = newFadeValue;
    }
}