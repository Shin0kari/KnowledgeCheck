using UnityEngine;

public class PlayerSpawnPosMarkerLinker : MonoBehaviour, IBindingSingletonComponent
{
    [field: SerializeField]
    public GameObject LinkerObject { get; private set; }

    private void Awake()
    {
        BindAllTypes();
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}