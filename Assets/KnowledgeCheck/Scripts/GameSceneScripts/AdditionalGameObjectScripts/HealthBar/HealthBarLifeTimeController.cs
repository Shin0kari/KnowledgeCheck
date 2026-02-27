using UnityEngine;

[RequireComponent(typeof(HealthBar))]
public class HealthBarLifeTimeController : MonoBehaviour
{
    [SerializeField] private GameObject _healthBarObject;

    public void OffHealthBar()
    {
        _healthBarObject.SetActive(false);
    }

    public void OnHealthBar()
    {
        _healthBarObject.SetActive(true);
    }

    public bool CheckHealthBarState()
    {
        return _healthBarObject.activeSelf;
    }
}