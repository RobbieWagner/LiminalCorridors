using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [SerializeField] private GameObject playerGO;
    [SerializeField] private NavMeshAgent navMeshAgent;

    [SerializeField] private float maxDistanceFromPlayer = 100f;

    private bool firstIdle;
    [SerializeField] private float initialWaitTime = 60f;
    [SerializeField] private float monsterCooldownTime = 10f;
    private bool standingIdle;
    [SerializeField] private int roamLimit;
    private bool roamingDestinationSet;
    private int pathCount;
    private bool roaming;
    private bool pausingIEnumerator;

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
        roaming = false;
        pausingIEnumerator = false;
    }

    // Update is called once per frame
    private void Update(){
        Vector3 monsterDestination;

        if(currentState == (int) monsterState.inactive && !standingIdle){
            if(firstIdle && !standingIdle){
                firstIdle = false;
                StartCoroutine(StandIdle(initialWaitTime));
            }
            else if(!standingIdle) StartCoroutine(StandIdle(monsterCooldownTime));
        }
        else if(!roamingDestinationSet && !roaming && currentState == (int) monsterState.roaming){
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
        //Debug.Log("looking for player");
        }
    }

    private void LookForPlayer(){
        if(IsPlayerVisible()){
            currentState = (int) monsterState.chasingPlayer;
            //Debug.Log("chasing");
        }
        else if(Vector3.Distance(navMeshAgent.destination, transform.position) < 1.0f){
            currentState = (int) monsterState.roaming;
            //Debug.Log("roaming");
        }
    }

    private IEnumerator Roam(){
        if(!roamingDestinationSet && !roaming)
        {
            float positionX = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
            float positionZ = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
            Vector3 path = new Vector3(playerGO.transform.position.x + positionX, 0, playerGO.transform.position.z + positionZ);
            while(!navMeshAgent.SetDestination(path)) {
                positionX = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                positionZ = Random.Range(-maxDistanceFromPlayer, maxDistanceFromPlayer);
                path = new Vector3(playerGO.transform.position.x + positionX, 0, playerGO.transform.position.z + positionZ);
            }
            pathCount++;
            roamingDestinationSet = true;
            roaming = true;
            //Debug.Log("Destination number " + pathCount);
        }

        while((!IsPlayerVisible()) && !(Vector3.Distance(navMeshAgent.destination, transform.position) < 1.0f)){
            yield return null;
        }
        
        if(IsPlayerVisible()){
            currentState = (int) monsterState.chasingPlayer;
            //Debug.Log("chasing");
        }

        if(pathCount == roamLimit){
            pathCount = 0;
            currentState = (int) monsterState.inactive;
            //Debug.Log("inactive");
        }

        roamingDestinationSet = false;
        roaming = false;
        StopCoroutine(Roam());
    }

    private IEnumerator StandIdle(float timeToStand){
        pausingIEnumerator = true;
        standingIdle = true;
        StartCoroutine(Pause(timeToStand));
        navMeshAgent.SetDestination(transform.position);
        while(!IsPlayerVisible() && pausingIEnumerator){
            yield return null;
        }
        if(IsPlayerVisible()) {
            currentState = (int) monsterState.chasingPlayer;
            //Debug.Log("chasing");
            standingIdle = false;
            StopCoroutine(StandIdle(timeToStand));
        }

        currentState = (int) monsterState.roaming;
        //Debug.Log("roaming");
        standingIdle = false;
        StopCoroutine(StandIdle(timeToStand));
    }

    private IEnumerator Pause(float timeToWait) {
        pausingIEnumerator = true;
        yield return new WaitForSeconds(timeToWait);
        pausingIEnumerator = false;
        StopCoroutine(Pause(timeToWait));
    }
}
