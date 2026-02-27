using UnityEngine;

public class SceneCharacterDataFiller : MonoBehaviour
{
    [SerializeField] private GameObject _playerSpawnPosMark;
    private Vector3 _newPosition;
    private Quaternion _newRotation;

    public (Vector3, Quaternion) FillPlayerPositionAndRotation(Vector3? position, Quaternion? rotation)
    {
        _newPosition = position ?? _playerSpawnPosMark.transform.position;
        _newRotation = rotation ?? Quaternion.identity;

        return (_newPosition, _newRotation);
    }

}