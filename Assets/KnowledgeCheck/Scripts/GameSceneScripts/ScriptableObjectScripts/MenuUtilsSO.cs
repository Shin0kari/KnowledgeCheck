using UnityEngine;

[CreateAssetMenu(fileName = "MenuUtils", menuName = "Scene Context/UI configs/Menu Configs/Menu Utils SO")]
public class MenuUtilsSO : ScriptableObject
{
    [field: SerializeField]
    public bool IsStopGameOnMenu
    {
        get;
        private set;
    }
}