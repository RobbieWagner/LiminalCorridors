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
    private float maxDistanceFromPlayer = 100f;

    private bool firstIdle;
    [SerializeField]
    private float initialWaitTime = 60f;
    [SerializeField]
    private float monsterCooldownTime = 10f;
    private bool standingIdle;
    [SerializeField]
    private int roamLimit;
    [SerializeField]
    private bool roamingDestinationSet;
    private int roamingPlaces;

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
        standingIdle = false;
        firstIdle = true;
        roamingDestinationSet = false;
    }

    // Update is called once per frame
    private void Update(){
        Vector3 monsterDestination;

        if(currentState == (int) monsterState.inactive && !standingIdle){
            if(firstIdle) {
                firstIdle = false;
                StartCoroutine(StandIdle(initialWaitTime));
            }
            else StartCoroutine(StandIdle(monsterCooldownTime));
        }
        else if(currentState == (int) monsterState.roaming){
            StartCoroutine(Roam());
        }
        else if(currentState == (int) monsterState.chasingPlayer){
            ChasePlayer();
        }
        else if(currentState == (int) monsterState.lookingForPlayer){
            LookForPlayer();
        }

    }

    private bool IsPlayerVisible(){
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(transform.position, playerGO.transform.position-transform.position);
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        if(hit.collider.gameObject.GetInstanceID() == playerGO.GetInstanceID()) return true;
        
        return false;
    } 

    private void ChasePlayer(){
        navMeshAgent.SetDestination(playerGO.transform.position);
        if(!IsPlayerVisible()){
        currentState = (int) monsterState.lookingForPlayer;
        Debug.Log("looking for player");
        }
    }

    private void LookForPlayer(){
        if(IsPlayerVisible()){
            currentState = (int) monsterState.chasingPlayer;
            Debug.Log("chasing");
        }
        else if(navMeshAgent.remainingDistance < .01f){
            currentState = (int) monsterState.roaming;
            Debug.Log("roaming");
        }
    }

    private IEnumerator Roam(){

        if(!roamingDestinationSet)
        {
            Debug.Log("finding new path");
            float positionX = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
            float positionZ = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
            Vector3 path = new Vector3(playerGO.transform.position.x + positionX, 0, playerGO.transform.position.z + positionZ);
            while(!navMeshAgent.SetDestination(path)) {
                positionX = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                positionZ = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                path = new Vector3(playerGO.transform.position.x + positionX, 0, playerGO.transform.position.z + positionZ);
            }
            roamingPlaces++;
            Debug.Log("path: " + path.ToString());
        }

        while(!IsPlayerVisible() && !(navMeshAgent.remainingDistance < 0.01f)){
            yield return null;
        }
        
        if(IsPlayerVisible()){
            currentState = (int) monsterState.chasingPlayer;
            Debug.Log("chasing");
        }

        if(roamingPlaces == roamLimit){
            roamingPlaces = 0;
            currentState = (int) monsterState.inactive;
            Debug.Log("inactive");
        }

        roamingDestinationSet = false;
        StopCoroutine(Roam());
    }

    private IEnumerator StandIdle(float timeToStand){
        standingIdle = true;
        navMeshAgent.SetDestination(transform.position);
        if(IsPlayerVisible()){
            yield return null;
            currentState = (int) monsterState.chasingPlayer;
            Debug.Log("chasing");
            standingIdle = false;
            StopCoroutine(StandIdle(timeToStand));
        }

        yield return new WaitForSeconds(timeToStand);
        currentState = (int) monsterState.roaming;
        Debug.Log("roaming");
        standingIdle = false;
        StopCoroutine(StandIdle(timeToStand));
    }
}
