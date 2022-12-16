using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private GameObject playerGO;
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private float maxDistanceFromPlayer;

    enum monsterState{
        inactive,
        roaming,
        chasingPlayer,
        lookingForPlayer
    }

    int currentState;

    // Start is called before the first frame update
    private void Start(){
        currentState = 0;
    }

    // Update is called once per frame
    private void Update(){
        Vector3 monsterDestination;

        if(currentState == (int) monsterState.inactive){
            if(IsPlayerVisible()){
            currentState = (int) monsterState.chasingPlayer;
            Debug.Log("Chasing");
            }
        }
        else if(currentState == (int) monsterState.roaming){
            Roam();
        }
        else if(currentState == (int) monsterState.chasingPlayer){
            navMeshAgent.SetDestination(playerGO.transform.position);
            if(!IsPlayerVisible()){
            currentState = (int) monsterState.lookingForPlayer;
            Debug.Log("looking for player");
            }
        }
        else if(currentState == (int) monsterState.lookingForPlayer){
            float distanceLeft = navMeshAgent.remainingDistance;
            if(IsPlayerVisible()){
                currentState = (int) monsterState.chasingPlayer;
            }
            else if(distanceLeft < .01f){
                currentState = (int) monsterState.roaming;
                Debug.Log("roaming");
            }
        }

    }

    private bool IsPlayerVisible (){
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(transform.position, playerGO.transform.position-transform.position);
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        if(hit.collider.gameObject.GetInstanceID() == playerGO.GetInstanceID()) return true;
        
        return false;
    } 

    private void Roam(){
        if(!navMeshAgent.hasPath)
        {
            Debug.Log("finding new path");
            float positionX = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
            float positionZ = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
            Vector3 path = new Vector3(playerGO.transform.position.x + positionX, 0, playerGO.transform.position.z + positionZ);
            navMeshAgent.SetDestination(path);
            Debug.Log("path: " + path.ToString());
        }
        if(IsPlayerVisible()){
            currentState = (int) monsterState.chasingPlayer;
        }
    }
}
