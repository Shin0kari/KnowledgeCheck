using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshUpdater : MonoBehaviour
{
    [SerializeField] private NavMeshSurface _navMeshSurface;

    public void UpdateNavMeshSurface()
    {
        _navMeshSurface.UpdateNavMesh(_navMeshSurface.navMeshData);
    }
}
