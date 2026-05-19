using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

public class HealthBarFactory : AbstractFactoryStarter, IDisposable
{
    private Dictionary<CharacterCanvasTypes, GameObject> _characterCanvasPrefabs = new();

    private IResourceLocationProvider _rLP;
    private IAddressablesProvider _aP;

    private IResourceLocation _characterCanvasRL;

    private HealthBar.Factory _healthBarFactory;

    private UniTaskCompletionSource _characterCanvasPrefabsLoadedsource = new();
    private CancellationTokenSource _ct = new();

    [Inject]
    private void Construct(
        IResourceLocationProvider rLP,
        IAddressablesProvider aP,
        HealthBar.Factory healthBarFactory
    )
    {
        _rLP = rLP;
        _aP = aP;
        _healthBarFactory = healthBarFactory;

        AsyncSetCharacterCanvasPrefabs().Forget();
    }

    public void Dispose()
    {
        _characterCanvasPrefabsLoadedsource.TrySetCanceled();
        _characterCanvasPrefabsLoadedsource = null;

        _ct?.Cancel();
        _ct?.Dispose();

        _characterCanvasPrefabs.Clear();
    }

    private async UniTaskVoid AsyncSetCharacterCanvasPrefabs()
    {
        foreach (CharacterType characterType in Enum.GetValues(typeof(CharacterType)))
        {
            if (!CharacterTypeToCharacterCanvasTypeParser(characterType, out CharacterCanvasTypes? canvasType))
                continue;

            _characterCanvasRL = await _rLP.AsyncGetUploadResourceLocation(canvasType.Value.ToString(), _ct.Token);
            _characterCanvasPrefabs.Add(canvasType.Value, await _aP.AsyncGetAddressablesDataFromLocation<GameObject>(_characterCanvasRL, _ct.Token));
        }

        _characterCanvasPrefabsLoadedsource.TrySetResult();
    }

    private bool CharacterTypeToCharacterCanvasTypeParser(CharacterType characterType, out CharacterCanvasTypes? healtBarTypes)
    {
        var isSuccess = true;
        healtBarTypes = null;
        switch (characterType)
        {
            case CharacterType.CommonEnemy:
                healtBarTypes = CharacterCanvasTypes.CommonEnemyCanvas;
                break;
            case CharacterType.Player:
                healtBarTypes = CharacterCanvasTypes.PlayerCanvas;
                break;
            default:
                isSuccess = false;
                break;
        }
        return isSuccess;
    }

    public async UniTaskVoid AsyncSpawnNotPlayableCharacterHealthBar(Enemy enemy)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, enemy.gameObject.GetCancellationTokenOnDestroy());
        await _characterCanvasPrefabsLoadedsource.Task.AttachExternalCancellation(linkedCTS.Token);

        if (!CharacterTypeToCharacterCanvasTypeParser(enemy.GetCharacterType(), out CharacterCanvasTypes? enemyCanvasType))
            ErrorMessageGenerator.GenerateSimpleError(this, "Invalid canvas specified");

        if (!_characterCanvasPrefabs.TryGetValue(enemyCanvasType.Value, out GameObject canvasPrefab))
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Health Canvas not found");
        }

        var enemyHealthBar = _healthBarFactory.Create(canvasPrefab);
        enemyHealthBar.transform.SetParent(enemy.transform);
        enemyHealthBar.transform.position = new(enemy.transform.position.x, enemyHealthBar.transform.position.y, enemy.transform.position.z);

        enemyHealthBar.SetDamagableObject(enemy);
    }

    public async UniTaskVoid SpawnPlayableCharacterHealthBarAsync(Player player)
    {
        using var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(_ct.Token, player.gameObject.GetCancellationTokenOnDestroy());
        await _characterCanvasPrefabsLoadedsource.Task.AttachExternalCancellation(linkedCTS.Token);

        if (!CharacterTypeToCharacterCanvasTypeParser(player.GetCharacterType(), out CharacterCanvasTypes? playerCanvasType))
            ErrorMessageGenerator.GenerateSimpleError(this, "Invalid canvas specified");

        if (!_characterCanvasPrefabs.TryGetValue(playerCanvasType.Value, out GameObject canvasPrefab))
        {
            ErrorMessageGenerator.GenerateSimpleError(this, "Health Canvas not found");
        }

        var playerHealthBar = _healthBarFactory.Create(canvasPrefab);

        playerHealthBar.SetDamagableObject(player);
    }
}
