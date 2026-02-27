using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class MiniGameController : MonoBehaviour, IDisposable
{
    [SerializeField] private ButtonItemGenerator _buttonItemGenerator;
    [SerializeField] private HoldMiniGameButton _miniGame;
    [SerializeField] private CongratulationPanel _congratulationsPanel;
    [SerializeField] private ButtonRechargeAnimation _buttonRechargeAnimation;

    [SerializeField] private GameObject _backgroundMiniGame;
    [SerializeField] private GameObject _rotatedMiniGameObject;

    private FloorItemSpawner _floorItemSpawner;
    private Vector3 _startMiniGamePos;

    [Inject]
    private void Construct(FloorItemSpawner floorItemSpawner)
    {
        _floorItemSpawner = floorItemSpawner;

        _startMiniGamePos = _rotatedMiniGameObject.transform.position;
    }

    private void Start()
    {
        _buttonItemGenerator.IsUsed += StartMiniGame;
        _miniGame.OnCompleteMiniGame += StartCongratulatons;
        _congratulationsPanel.OnShowCompleteText += SetRechargeButtonState;
        _congratulationsPanel.OnShowCompleteText += SpawnItem;
        _buttonRechargeAnimation.OnFullCharge += SetDefaultButtonState;
    }

    public void Dispose()
    {
        _buttonItemGenerator.IsUsed -= StartMiniGame;
        _miniGame.OnCompleteMiniGame -= StartCongratulatons;
        _congratulationsPanel.OnShowCompleteText -= SetRechargeButtonState;
        _congratulationsPanel.OnShowCompleteText -= SpawnItem;
        _buttonRechargeAnimation.OnFullCharge -= SetDefaultButtonState;
    }

    private void StartMiniGame()
    {
        SetMiniGameDefaultSetting();

        _buttonItemGenerator.DisableButton();
        _backgroundMiniGame.SetActive(true);
        _miniGame.gameObject.SetActive(true);
    }

    private void SetMiniGameDefaultSetting()
    {
        _rotatedMiniGameObject.transform.position = _startMiniGamePos;
        _rotatedMiniGameObject.transform.rotation = Quaternion.identity;
    }

    private void StartCongratulatons()
    {
        _miniGame.gameObject.SetActive(false);
        _backgroundMiniGame.SetActive(false);
        _congratulationsPanel.gameObject.SetActive(true);
    }

    private void SetRechargeButtonState()
    {
        _congratulationsPanel.gameObject.SetActive(false);
        _buttonRechargeAnimation.StartRecharge();
    }

    private void SpawnItem()
    {
        _floorItemSpawner.SpawnItem();
    }

    private void SetDefaultButtonState()
    {
        _buttonItemGenerator.EnableButton();
    }
}