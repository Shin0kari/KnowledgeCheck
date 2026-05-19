using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

public class SceneCharacterDataFiller : IDisposable
{
    private IAssetProviderGetter _assetProvider;

    private GameObject _playerSpawnPosMark;

    private Vector3 _newPosition;
    private Quaternion _newRotation;

    private DisposableBag _dB;
    private UniTaskCompletionSource _spawnPosReady = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(IAssetProviderGetter assetProvider)
    {
        _assetProvider = assetProvider;

        SubscribeOnUpdateObjects();
    }

    public void Dispose()
    {
        _spawnPosReady?.TrySetCanceled();
        _spawnPosReady = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<PlayerSpawnPosMarkerLinker>()
            .OfType<IBindingSingletonComponent, PlayerSpawnPosMarkerLinker>()
            .Subscribe(playerSpawnPosMarkerLinker =>
            {
                if (playerSpawnPosMarkerLinker == null)
                    return;

                _playerSpawnPosMark = playerSpawnPosMarkerLinker.LinkerObject;
                _spawnPosReady.TrySetResult();
            })
            .AddTo(ref _dB);
    }

    public async UniTask<(Vector3, Quaternion)> FillPlayerPositionAndRotation(Vector3? position, Quaternion? rotation, CancellationToken ct)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, ct);
        await _spawnPosReady.Task.AttachExternalCancellation(cancellationToken: linkedCTS.Token);

        _newPosition = _playerSpawnPosMark.transform.position;
        _newRotation = Quaternion.identity;

        return (_newPosition, _newRotation);
    }
}