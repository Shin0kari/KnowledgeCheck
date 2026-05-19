using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;

public class UpdateSaveFromInventory : IDisposable
{
    private PlayableCharacterDataUpdater _characterDataUpdater;

    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(PlayableCharacterDataUpdater characterDataUpdater)
    {
        _characterDataUpdater = characterDataUpdater;
    }

    public void Dispose()
    {
        _ct?.Cancel();
        _ct?.Dispose();
    }

    public async UniTask FillGameDataFromInventory()
    {
        await _characterDataUpdater.UpdateCharacterData();
    }
}