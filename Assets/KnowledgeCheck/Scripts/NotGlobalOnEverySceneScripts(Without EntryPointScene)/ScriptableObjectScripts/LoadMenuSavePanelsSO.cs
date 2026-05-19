using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "LoadMenuSavePanelsSO", menuName = "Scene Context/NotEnrtyPointSOs/LoadMenu SavePanels SO")]
public class LoadMenuSavePanelsSO : ScriptableObject
{
    [field: SerializeField]
    public AssetReferenceT<GameObject> LoadPanel
    {
        get;
        private set;
    }
    [field: SerializeField]
    public AssetReferenceT<GameObject> NewGamePanel
    {
        get;
        private set;
    }
}