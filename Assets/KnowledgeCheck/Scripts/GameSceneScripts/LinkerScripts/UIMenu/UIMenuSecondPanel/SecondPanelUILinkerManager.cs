using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using Zenject;

public class SecondPanelUILinkerManager : IDisposable
{
    private IAssetProviderGetter _assetProvider;
    private HashSet<AbstractSecondUIPanelLinker> _secondPanelLinkers = new();
    private bool _isStartMenuPanelFound = false;

    private DisposableBag _dB;

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        DisposeBags();
        DisposeDynamicProperty();
    }

    private void DisposeBags()
    {
        _dB.Dispose();
    }

    private void DisposeDynamicProperty()
    {
        _secondPanelLinkers.Clear();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingTransientComponent<AbstractSecondUIPanelLinker>()
            .Subscribe((secondPanelLinkers) =>
            {
                if (secondPanelLinkers == null || secondPanelLinkers.Count < 1)
                    return;
                UpdateUI(secondPanelLinkers);
            })
            .AddTo(ref _dB);
    }

    private void UpdateUI(List<IBindingTransientComponent> secondPanelLinkers)
    {
        foreach (AbstractSecondUIPanelLinker secondPanelLinker in secondPanelLinkers.Cast<AbstractSecondUIPanelLinker>())
        {
            if (!_secondPanelLinkers.Contains(secondPanelLinker))
            {
                UpdateUIDataToCurrent(secondPanelLinker);
                if (!_isStartMenuPanelFound)
                {
                    if (secondPanelLinker is IStartMenuPanel startMenuPanel)
                    {
                        startMenuPanel.ActivatePanel();
                    }
                }
            }
        }
    }

    private void UpdateUIDataToCurrent(AbstractSecondUIPanelLinker secondPanelLinker)
    {
        _secondPanelLinkers.Add(secondPanelLinker);

        secondPanelLinker.LinkerObject.SetActive(false);
    }

    public void OffAllPanels()
    {
        foreach (var linker in _secondPanelLinkers)
        {
            linker.gameObject.SetActive(false);
        }
    }
}