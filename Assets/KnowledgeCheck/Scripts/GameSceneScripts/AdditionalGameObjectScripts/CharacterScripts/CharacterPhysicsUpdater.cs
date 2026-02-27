using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterPhysicsUpdater : MonoBehaviour
{
    private bool _isUnderWater;

    private Rigidbody _rb;
    private float _standartWaterDrag;
    private float _standartWaterAngularDrag;

    [SerializeField] private float _waterDrag = 3f;
    [SerializeField] private float _waterAngularDrag = 1f;
    // [SerializeField] private float _waterDensity = 0.3f;

    [SerializeField] private HeadBoxScript _headBox;

    public event Action<bool> OnUnderWaterStateChange;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _standartWaterDrag = _rb.linearDamping;
        _standartWaterAngularDrag = _rb.angularDamping;

        _headBox.ChangeUnderWaterState += ChangeUnderWaterState;
    }

    public void Dispose()
    {
        _headBox.ChangeUnderWaterState -= ChangeUnderWaterState;
    }

    // private void Update()
    // {
    //     if (_isUnderWater)
    //         _rb.AddForceAtPosition(Vector3.up * Mathf.Abs(Physics.gravity.y) * _waterDensity, transform.position, ForceMode.Acceleration);

    // }

    private void ChangeUnderWaterState(bool currentState)
    {
        if (currentState != _isUnderWater)
        {
            _isUnderWater = currentState;
            ChangePhysicsParameters(_isUnderWater);

            OnUnderWaterStateChange?.Invoke(_isUnderWater);
        }
    }

    private void ChangePhysicsParameters(bool isUnderWater)
    {
        if (isUnderWater)
        {
            _rb.linearDamping = _waterDrag;
            _rb.angularDamping = _waterAngularDrag;
        }
        else
        {
            _rb.linearDamping = _standartWaterDrag;
            _rb.angularDamping = _standartWaterAngularDrag;
        }
    }
}