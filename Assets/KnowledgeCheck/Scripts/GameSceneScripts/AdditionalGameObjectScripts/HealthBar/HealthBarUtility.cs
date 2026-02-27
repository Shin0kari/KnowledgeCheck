using UnityEngine;
using UnityEngine.UI;

public class HealthBarUtility : MonoBehaviour
{
    [SerializeField] private Image _healthFilled;

    public Image GetHealthFilled()
    {
        return _healthFilled;
    }
}