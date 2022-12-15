using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour
{
    public void buildNavMeshes(List<NavMeshSurface> navMeshSurfaces){
        foreach(NavMeshSurface nms in navMeshSurfaces) {
            nms.BuildNavMesh();
        }
    }
}
