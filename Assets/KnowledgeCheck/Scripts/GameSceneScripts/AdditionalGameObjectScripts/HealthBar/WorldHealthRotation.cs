using UnityEngine;
using Zenject;

public class WorldHealthRotation : MonoBehaviour
{
    private float _canvasRotationSpeed = 10f;
    private Vector3 direction;
    private Quaternion lookRotation;
    private bool _isReady = false;

    private CameraUtils _cameraUtils;

    public void Construct(
        float rotationSpeed,
        CameraUtils cameraUtils)
    {
        _cameraUtils = cameraUtils;
        _canvasRotationSpeed = rotationSpeed;

        _isReady = true;
    }

    private void LateUpdate()
    {
        if (!_isReady)
            return;

        // direction = _cameraUtils.transform.position - transform.position;

        // lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        // transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * _canvasRotationSpeed);
        transform.rotation = _cameraUtils.transform.rotation;
    }

}