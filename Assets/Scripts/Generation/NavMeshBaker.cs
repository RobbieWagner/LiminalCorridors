using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    public void BuildNavMeshes(List<NavMeshSurface> navMeshSurfaces){
        foreach(NavMeshSurface nms in navMeshSurfaces) {
            nms.BuildNavMeshAsync();
        }
    }

    public void UpdateNavMeshes(List<NavMeshSurface> navMeshSurfaces){
        //NavMeshBuilder.UpdateNavMeshDataAsync();
    }
}
