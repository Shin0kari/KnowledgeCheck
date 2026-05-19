using UnityEngine;

public class FloorItemLinker : MonoBehaviour, IBindingTransientComponent
{
    [field: SerializeField]
    public InventoryItem LinkerObject { get; private set; }

    private void Awake()
    {
        BindAllTypes();
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}