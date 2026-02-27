using UnityEngine;
using UnityEngine.InputSystem;

public class CursorVisibiliter : MonoBehaviour
{
    [SerializeField] private bool ShouldEnableOnStartCursorVisibility;

    private void Awake()
    {
        if (ShouldEnableOnStartCursorVisibility)
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