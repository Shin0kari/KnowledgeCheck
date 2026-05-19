using UnityEngine;
using UnityEngine.InputSystem;

public class CursorVisibiliter : MonoBehaviour
{
    [SerializeField] private bool _shouldEnableOnStartCursorVisibility;

    private void Awake()
    {
        if (_shouldEnableOnStartCursorVisibility)
        {
            CursorVisibility.OffAlwaysCursorVisibility();
            CursorVisibility.OnCursorVisibility();
        }
        else
        {
            CursorVisibility.OffAlwaysCursorVisibility();
            CursorVisibility.OffCursorVisibility();
        }
    }

    private void OnDestroy()
    {
        CursorVisibility.OffAlwaysCursorVisibility();
        CursorVisibility.OffCursorVisibility();
    }
}