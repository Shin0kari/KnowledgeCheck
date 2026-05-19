using UnityEngine;

public class ParentContainerInventoryPanelLinker : MonoBehaviour, IBindingSingletonComponent
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