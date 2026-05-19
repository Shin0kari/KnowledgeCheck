using UnityEngine;

public class MainPanelLinker : MonoBehaviour, IBindingSingletonComponent
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