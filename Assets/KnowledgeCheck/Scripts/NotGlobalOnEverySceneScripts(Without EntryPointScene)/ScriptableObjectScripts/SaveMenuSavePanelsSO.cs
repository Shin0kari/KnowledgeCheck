using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SaveMenuSavePanelsSO", menuName = "Scene Context/NotEnrtyPointSOs/SaveMenu SavePanels SO")]
public class SaveMenuSavePanelsSO : ScriptableObject
{
    [field: SerializeField]
    public AssetReferenceT<GameObject> SavePanel
    {
        get;
        private set;
    }
    [field: SerializeField]
    public AssetReferenceT<GameObject> NewSavePanel
    {
        get;
        private set;
    }
}