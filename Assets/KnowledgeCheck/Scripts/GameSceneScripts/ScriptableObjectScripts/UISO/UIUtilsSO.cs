using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "UIUtilsSO", menuName = "Scene Context/UI configs/UI Utils SO")]
public class UIUtilsSO : ScriptableObject
{
    // public AssetReferenceT<GameObject> WinPanelSO { get { return _winPanelSO; } }
    // [SerializeField] private AssetReferenceT<GameObject> _winPanelSO;

    // public AssetReferenceT<GameObject> WhitePanelSO { get { return _whitePanelSO; } }
    // [SerializeField] private AssetReferenceT<GameObject> _whitePanelSO;

    // public AssetReferenceT<GameObject> LosePanelSO { get { return _losePanelSO; } }
    // [SerializeField] private AssetReferenceT<GameObject> _losePanelSO;

    // public AssetReferenceT<GameObject> RedPanelSO { get { return _redPanelSO; } }
    // [SerializeField] private AssetReferenceT<GameObject> _redPanelSO;

    // public AssetReferenceT<GameObject> BluePanelSO { get { return _bluePanelSO; } }
    // [SerializeField] private AssetReferenceT<GameObject> _bluePanelSO;

    public Ease WhiteWindowAnimCurve { get { return _whiteWindowAnimCurve; } }
    [SerializeField] private Ease _whiteWindowAnimCurve = Ease.InOutQuart;

    public float MaxColorCanvasFade { get { return _maxColorCanvasFade; } }
    [SerializeField] private float _maxColorCanvasFade = 0.5f;

    public float ColorCanvasFadeDuration { get { return _colorCanvasFadeDuration; } }
    [SerializeField] private float _colorCanvasFadeDuration = 0.5f;

    public float LoseTextFadeDuration { get { return _loseTextFadeDuration; } }
    [SerializeField] private float _loseTextFadeDuration = 0.5f;

}