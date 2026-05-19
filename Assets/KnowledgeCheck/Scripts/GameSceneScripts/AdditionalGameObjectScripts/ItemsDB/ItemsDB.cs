using System.Collections.Generic;
using UnityEngine;

public class ItemsDB : MonoBehaviour, IBindingSingletonComponent
{
    public List<ItemSO> allItemsSO;

    private void Awake()
    {
        BindAllTypes();
    }

    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}