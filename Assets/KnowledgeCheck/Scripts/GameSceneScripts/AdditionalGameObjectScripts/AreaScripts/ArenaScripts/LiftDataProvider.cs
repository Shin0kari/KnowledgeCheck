using UnityEngine;

public class LiftDataProvider : MonoBehaviour, IBindingSingletonComponent
{
    [SerializeField] private GameObject _liftPosMark;

    [SerializeField] private GameObject _northArenaWall;
    [SerializeField] private GameObject _eastArenaWall;
    [SerializeField] private GameObject _southArenaWall;
    [SerializeField] private GameObject _westArenaWall;

    public float LiftYPos
    {
        get
        {
            return _liftPosMark.transform.position.y;
        }
    }

    public Vector3 NorthArenaWallPos
    {
        get
        {
            return _northArenaWall.transform.position;
        }
    }
    public Vector3 EastArenaWallPos
    {
        get
        {
            return _eastArenaWall.transform.position;
        }
    }
    public Vector3 SouthArenaWallPos
    {
        get
        {
            return _southArenaWall.transform.position;
        }
    }
    public Vector3 WestArenaWallPos
    {
        get
        {
            return _westArenaWall.transform.position;
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