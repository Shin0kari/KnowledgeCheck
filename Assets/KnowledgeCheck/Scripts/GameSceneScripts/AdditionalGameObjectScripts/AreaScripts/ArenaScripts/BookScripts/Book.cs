using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class Book : MonoBehaviour
{
    [SerializeField] private BookAnimation _bookAnimation;

    private IAssetProviderGetter _assetProvider;
    private ArenaController _arenaController;

    private List<GameObject> _targetObjects = new(); // На случай онлайна
    private GameObject _newTarget;
    private StarterArenaEvent _starter;

    private bool _isBattleStarted = false;
    private Rigidbody _rigidbody;

    private DisposableBag _dB;

    [Inject]
    private void Construct(
        IAssetProviderGetter assetProvider,
        ArenaController arenaController)
    {
        _assetProvider = assetProvider;
        _arenaController = arenaController;

        _rigidbody = GetComponent<Rigidbody>();
        _arenaController.StartArenaBattle += OnBattleStarted;

        SubscribeOnUpdateObjects();
    }

    private void OnDestroy()
    {
        if (_starter != null)
        {
            _starter.PlayerOnStarter -= UpdateBook;
            _starter.PlayerLeftStarter -= UpdateBook;
        }
        if (_arenaController != null)
            _arenaController.StartArenaBattle -= OnBattleStarted;
        _targetObjects.Clear();

        _dB.Dispose();
    }

    private void SubscribeOnUpdateObjects()
    {
        if (_assetProvider == null)
            ErrorMessageGenerator.GenerateSimpleError(this, "Asset provider not set");

        _assetProvider
            .GetIBindingSingletonComponent<StarterArenaEvent>()
            .OfType<IBindingSingletonComponent, StarterArenaEvent>()
            .Subscribe(starter =>
            {
                if (starter == null)
                    return;

                if (_starter != null)
                {
                    _starter.PlayerOnStarter -= UpdateBook;
                    _starter.PlayerLeftStarter -= UpdateBook;
                }

                _starter = starter;
                _starter.PlayerOnStarter += UpdateBook;
                _starter.PlayerLeftStarter += UpdateBook;

            })
            .AddTo(ref _dB);
    }


    private void UpdateBook(Player player)
    {
        if (_isBattleStarted)
            return;

        if (_targetObjects.Contains(player.gameObject))
            _targetObjects.Remove(player.gameObject);
        else
            _targetObjects.Add(player.gameObject);

        UpdateCurrentBookTarget();
    }

    private void OnBattleStarted()
    {
        _isBattleStarted = true;
        _rigidbody.constraints = RigidbodyConstraints.None;
        _bookAnimation.OffBookAnimation();
    }

    private void UpdateCurrentBookTarget()
    {
        if (_targetObjects.Count > 0)
            _newTarget = _targetObjects[0];
        else
            _newTarget = null;

        _bookAnimation.SetNewTargetObject(_newTarget);
    }
}