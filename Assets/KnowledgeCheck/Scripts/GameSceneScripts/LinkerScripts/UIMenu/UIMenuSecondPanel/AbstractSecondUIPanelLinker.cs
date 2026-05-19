using UnityEngine;

public abstract class AbstractSecondUIPanelLinker : MonoBehaviour, IBindingTransientComponent
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