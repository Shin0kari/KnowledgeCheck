using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonRechargeAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonName;
    [SerializeField] private TextMeshProUGUI _chargeText;
    [SerializeField] private Image _chargeImage;
    [SerializeField] private float _maxRechargeDuation = 30f;
    [SerializeField] private float _secondsUpdateTime = 1f;
    private float _currentCharge;

    public event Action OnFullCharge;

    private void Awake()
    {
        SetStartData();
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

        }
    }

    private void ChangeDisplayedRechargeFeatures()
    {
        bool currentButtonNameActive = _buttonName.gameObject.activeSelf;
        _buttonName.gameObject.SetActive(!currentButtonNameActive);
        _chargeImage.gameObject.SetActive(currentButtonNameActive);
        _chargeText.gameObject.SetActive(currentButtonNameActive);
    }

    private async UniTask AsyncUpdateCharge(CancellationToken token)
    {
        _currentCharge = _maxRechargeDuation;

        while (_currentCharge >= 0f)
        {
            _chargeText.text = _currentCharge.ToString();
            float targetAmount = _currentCharge / _maxRechargeDuation;

            await _chargeImage.DOFillAmount(targetAmount, _secondsUpdateTime).ToUniTask(TweenCancelBehaviour.Kill, token);

            _currentCharge -= _secondsUpdateTime;
        }
    }

    private void SendFullChargeSignal()
    {
        OnFullCharge?.Invoke();
    }
}