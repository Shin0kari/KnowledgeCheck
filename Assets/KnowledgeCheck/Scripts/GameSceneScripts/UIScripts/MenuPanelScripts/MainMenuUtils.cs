using UnityEngine;

[CreateAssetMenu(fileName = "MenuUtils", menuName = "MenuConfigs/MenuUtils")]
public class MainMenuUtils : ScriptableObject
{
    [field: SerializeField]
    public bool IsStopGameOnMenu
    {
        get;
        private set;
    }
}