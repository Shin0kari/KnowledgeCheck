using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRechargeAnimation : MonoBehaviour, IBindingSingletonComponent
{
    [SerializeField] private TextMeshProUGUI _buttonName;
    [SerializeField] private TextMeshProUGUI _chargeText;
    [SerializeField] private Image _chargeImage;
    [SerializeField] private float _maxRechargeDuation = 30f;
    [SerializeField] private float _secondsUpdateTime = 1f;
    private float _currentCharge;
    private float _targetAmount;
    private bool _currentButtonNameActive;

    public event Action OnFullCharge;

    private void Awake()
    {
        SetStartData();
    }

    private void OnDestroy()
    {
        OnFullCharge = null;
    }

    private void SetStartData()
    {
        if (_secondsUpdateTime > _maxRechargeDuation)
            _secondsUpdateTime = _maxRechargeDuation;
    }

    public void StartRecharge()
    {
        StartAsyncUpdateCharge(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask StartAsyncUpdateCharge(CancellationToken token)
    {
        try
        {
            ChangeDisplayedRechargeFeatures();

            await AsyncUpdateCharge(token);

            ChangeDisplayedRechargeFeatures();
            SendFullChargeSignal();
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
    }

    private void ChangeDisplayedRechargeFeatures()
    {
        _currentButtonNameActive = _buttonName.gameObject.activeSelf;
        _buttonName.gameObject.SetActive(!_currentButtonNameActive);
        _chargeImage.gameObject.SetActive(_currentButtonNameActive);
        _chargeText.gameObject.SetActive(_currentButtonNameActive);
    }

    private async UniTask AsyncUpdateCharge(CancellationToken token)
    {
        _currentCharge = _maxRechargeDuation;

        while (_currentCharge >= 0f)
        {
            _chargeText.text = _currentCharge.ToString();
            _targetAmount = _currentCharge / _maxRechargeDuation;

            await _chargeImage.DOFillAmount(_targetAmount, _secondsUpdateTime).ToUniTask(TweenCancelBehaviour.Kill, token);

            _currentCharge -= _secondsUpdateTime;

            await UniTask.Yield(cancellationToken: token);
        }
    }

    private void SendFullChargeSignal()
    {
        OnFullCharge?.Invoke();
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}