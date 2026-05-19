using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Animator))]
public class LoadingScreenView : MonoBehaviour, IBindingSingletonComponent
{
    [SerializeField] private GameObject _loadScreen;
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Image _progressBar;

    private LoadingScreenController _loadingScreenController;

    public readonly ReactiveProperty<bool> IsReady = new() { Value = false };

    [Inject]
    private void Construct(LoadingScreenController loadingScreenController)
    {
        _loadingScreenController = loadingScreenController;

        BindAllTypes();

        _loadingScreenController.OnProgressChanged += SetBarProgress;
        _loadingScreenController.OnStartAnimation += PlayStartAnimation;
        _loadingScreenController.OnEndAnimation += PlayEndAnimation;
    }

    private void OnDestroy()
    {
        if (_loadingScreenController != null)
        {
            _loadingScreenController.OnProgressChanged -= SetBarProgress;
            _loadingScreenController.OnStartAnimation -= PlayStartAnimation;
            _loadingScreenController.OnEndAnimation -= PlayEndAnimation;
        }

        IsReady?.Dispose();
    }

    private void Start()
    {
        _animator.Play("Idle");
        IsReady.Value = true;
    }

    public void PlayStartAnimation()
    {
        if (_loadScreen == null)
        {
            Debug.LogError("PlayStartAnim: _loadScreen is NULL!");
            return;
        }
        if (_animator == null)
        {
            Debug.LogError("PlayStartAnim: _animator is NULL!");
            return;
        }
        _loadScreen.SetActive(true);
        if (_animator.runtimeAnimatorController == null)
        {
            Debug.LogError("PlayStartAnim: Animator Controller is missing!");
            return;
        }
        _animator.SetTrigger("loadStart");
    }

    public void PlayEndAnimation()
    {
        _progressText.text = 100 + " %";
        _progressBar.fillAmount = 1f;
        _loadScreen.SetActive(true);

        _animator.SetTrigger("loadEnd");
    }

    public void SetBarProgress(float value)
    {
        _progressText.text = Mathf.RoundToInt(value * 100) + " %";
        _progressBar.fillAmount = value;
    }

    public void OnStartLoadAnimationOver()
    {
        _loadingScreenController.OnStartLoadAnimationOver();
    }

    public void OnEndLoadAnimationOver()
    {
        _loadScreen.SetActive(false);
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}
