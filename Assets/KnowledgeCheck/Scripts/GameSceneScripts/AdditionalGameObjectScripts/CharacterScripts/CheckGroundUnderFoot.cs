using UnityEngine;

public class CheckGroundUnderFoot : MonoBehaviour
{
    [SerializeField] private float _rayLenght = 1f;
    private RaycastHit _hit;
    private LayerMask _groundLayerMasks;

    private string _tag;

    private void Awake()
    {
        _groundLayerMasks = LayerMask.GetMask("Walkable", "Solid");
    }

    public GroundType CheckGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, _rayLenght, _groundLayerMasks))
        {
            _tag = _hit.collider.gameObject.tag;
            if (System.Enum.TryParse(_tag, out GroundType result))
            {
                return result;
            }
        }

        return GroundType.Nothing;
    }
}

public enum GroundType
{
    Nothing,
    Stone,
    Metal,
}