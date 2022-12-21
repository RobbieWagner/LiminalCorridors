using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReorderTiles : MonoBehaviour
{
    [SerializeField] private Grid parentGrid;
    [SerializeField] private int quadrant;
    [SerializeField] private GameObject player;

    private void Start() {
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.GetInstanceID() == player.GetInstanceID()) {
            //Debug.Log("quadrant" + quadrant + " on " + parentGrid.gameObject.name);
            if(quadrant == 1) {
                parentGrid.gridOnLeft.position = new Vector3(parentGrid.transform.position.x + 400, 1, parentGrid.transform.position.z + 400);
                parentGrid.gridOnRight.position = new Vector3(parentGrid.transform.position.x + 400, 1, parentGrid.transform.position.z);
                parentGrid.gridAbove.position = new Vector3(parentGrid.transform.position.x, 1, parentGrid.transform.position.z + 400);
                parentGrid.gridBelow.position = new Vector3(parentGrid.transform.position.x - 400, 1, parentGrid.transform.position.z + 400);
            }
            else if(quadrant == 2) {
                parentGrid.gridOnLeft.position = new Vector3(parentGrid.transform.position.x - 400, 1, parentGrid.transform.position.z);
                parentGrid.gridOnRight.position = new Vector3(parentGrid.transform.position.x - 400, 1, parentGrid.transform.position.z - 400);
                parentGrid.gridAbove.position = new Vector3(parentGrid.transform.position.x, 1, parentGrid.transform.position.z + 400);
                parentGrid.gridBelow.position = new Vector3(parentGrid.transform.position.x - 400, 1, parentGrid.transform.position.z + 400);
            }
            else if(quadrant == 3) {
                parentGrid.gridOnLeft.position = new Vector3(parentGrid.transform.position.x - 400, 1, parentGrid.transform.position.z);
                parentGrid.gridOnRight.position = new Vector3(parentGrid.transform.position.x - 400, 1, parentGrid.transform.position.z - 400);
                parentGrid.gridAbove.position = new Vector3(parentGrid.transform.position.x + 400, 1, parentGrid.transform.position.z - 400);
                parentGrid.gridBelow.position = new Vector3(parentGrid.transform.position.x, 1, parentGrid.transform.position.z - 400);
            }
            else if(quadrant == 4) {
                parentGrid.gridOnLeft.position = new Vector3(parentGrid.transform.position.x + 400, 1, parentGrid.transform.position.z + 400);
                parentGrid.gridOnRight.position = new Vector3(parentGrid.transform.position.x + 400, 1, parentGrid.transform.position.z);
                parentGrid.gridAbove.position = new Vector3(parentGrid.transform.position.x + 400, 1, parentGrid.transform.position.z - 400);
                parentGrid.gridBelow.position = new Vector3(parentGrid.transform.position.x, 1, parentGrid.transform.position.z - 400);
            }

            parentGrid.navMeshBaker.BuildNavMeshes(parentGrid.navMeshSurfaces);

            //NavMeshBuildSettings nmbs = NavMesh.GetSettingsByID(0);
            //parentGrid.navMeshBaker.UpdateNavMeshes(parentGrid.navMeshData, nmbs, );
        }
    }
}
