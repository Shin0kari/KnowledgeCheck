using UnityEngine;

public class CameraTrigger : MonoBehaviour, ICameraService
{
    [SerializeField] private Animator _cameraAnimator;

    public void SetCameraTrigger(CameraTypes type)
    {
        _cameraAnimator.SetTrigger(type.ToString());
    }
}