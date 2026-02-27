using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Animator))]
public class LoadingScreenView : MonoBehaviour
{
    [SerializeField] private GameObject _loadScreen;
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI _progressText;
    [SerializeField] private Image _progressBar;

    private LoadingScreenController _loadingScreenController;

    [Inject]
    private void Construct(LoadingScreenController loadingScreenController)
    {
        if (!gameObject.activeSelf)
            return;

        _loadingScreenController = loadingScreenController;

        _loadingScreenController.OnProgressChanged += SetBarProgress;
        _loadingScreenController.OnStartAnimation += PlayStartAnimation;
        _loadingScreenController.OnEndAnimation += PlayEndAnimation;
    }

    private void Start()
    {
        _animator.Play("Idle");
    }

    private void OnDestroy()
    {
        if (_loadingScreenController != null)
        {
            _loadingScreenController.OnProgressChanged -= SetBarProgress;
            _loadingScreenController.OnStartAnimation -= PlayStartAnimation;
            _loadingScreenController.OnEndAnimation -= PlayEndAnimation;
        }
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
}
