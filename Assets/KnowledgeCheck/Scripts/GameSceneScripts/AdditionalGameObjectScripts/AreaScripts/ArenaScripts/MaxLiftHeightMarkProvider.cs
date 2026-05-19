using UnityEngine;

public class MaxLiftHeightMarkProvider : MonoBehaviour, IBindingSingletonComponent
{
    [SerializeField] private GameObject _maxYLiftHeightMark;

    public float MaxYLiftHeight
    {
        get
        {
            return _maxYLiftHeightMark.transform.position.y;
        }
    }

    private void Awake()
    {
        BindAllTypes();
    }
    public void BindAllTypes()
    {
        TypeCache.GetRelatedTypes(GetType());
    }
}