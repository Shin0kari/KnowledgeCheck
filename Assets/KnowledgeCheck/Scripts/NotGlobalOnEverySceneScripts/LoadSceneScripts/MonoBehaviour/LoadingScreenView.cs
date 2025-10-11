using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        _loadingScreenController = loadingScreenController;
    }

    private void Awake()
    {
        _loadingScreenController.OnProgressChanged += SetBarProgress;
        _loadingScreenController.OnStartAnimation += PlayStartAnimation;
        _loadingScreenController.OnEndAnimation += PlayEndAnimation;
    }

    private void OnDestroy()
    {
        _loadingScreenController.OnProgressChanged -= SetBarProgress;
        _loadingScreenController.OnStartAnimation -= PlayStartAnimation;
        _loadingScreenController.OnEndAnimation -= PlayEndAnimation;
    }

    public void PlayStartAnimation()
    {
        _loadScreen.SetActive(true);
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
