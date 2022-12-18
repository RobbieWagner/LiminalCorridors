using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [SerializeField] private GameObject playerGO;
    [SerializeField] private Transform cameraHolder;
    private CameraController playerCameraController;
    private Movement playerMovement;
    [SerializeField] private NavMeshAgent navMeshAgent;

    [SerializeField] private float maxDistanceFromPlayer = 100f;

    private bool firstIdle;
    [SerializeField] private float initialWaitTime = 60f;
    [SerializeField] private float monsterCooldownTime = 10f;
    private bool standingIdle;
    [SerializeField] private int roamLimit;
    private bool canSeePlayer;
    private bool roamingDestinationSet;
    private int pathCount;
    private bool roaming;
    private bool chasing;
    private bool pausingIEnumerator;
    [SerializeField, Range(.5f, 5f)] private float noticePlayerTime;

    private Coroutine pauseStandIdle;
    private bool noticingPlayer;

    [SerializeField] Transform head;
    Quaternion initialHeadRotation;

    public enum monsterState{
        inactive,
        roaming,
        chasingPlayer,
        lookingForPlayer
    }

    public int currentState;

    // Start is called before the first frame update
    private void Start(){
        currentState = 0;
        standingIdle = false;
        firstIdle = true;
        roamingDestinationSet = false;
        roaming = false;
        pausingIEnumerator = false;
        noticingPlayer = false;
        chasing = false;

        initialHeadRotation = head.rotation;

        playerMovement = playerGO.GetComponent<Movement>();
        playerCameraController = cameraHolder.GetComponent<CameraController>();
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

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.GetInstanceID() == playerGO.GetInstanceID()) canSeePlayer = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.GetInstanceID() == playerGO.GetInstanceID()) canSeePlayer = false;
    }

    private void ChasePlayer(){

        if(!chasing && !standingIdle) StartCoroutine(StandIdle(noticePlayerTime));

        if(chasing){
            head.LookAt(playerGO.transform.position);
            navMeshAgent.SetDestination(playerGO.transform.position);
            if(!IsPlayerVisible()){
            currentState = (int) monsterState.lookingForPlayer;
            //Debug.Log("looking for player");
            }
        }
    }

    private void LookForPlayer(){
        if(IsPlayerVisible() && canSeePlayer){
            noticingPlayer = true;
            currentState = (int) monsterState.chasingPlayer;
            //Debug.Log("chasing");
        }
        else if(Vector3.Distance(navMeshAgent.destination, transform.position) < 1.0f){
            chasing = false;
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

        while((!IsPlayerVisible() || !canSeePlayer) && !(Vector3.Distance(navMeshAgent.destination, transform.position) < 1.0f)){
            yield return null;
        }

        if(pathCount == roamLimit){
            pathCount = 0;
            currentState = (int) monsterState.inactive;
            //Debug.Log("inactive");
        }
        
        if(IsPlayerVisible() && canSeePlayer){
            noticingPlayer = true;
            currentState = (int) monsterState.chasingPlayer;
            //Debug.Log("chasing");
            roaming = false;
            playerCameraController.canLookAround = false;
            playerMovement.canMove = false;
            StopCoroutine(Roam());
        }

        roamingDestinationSet = false;
        roaming = false;
        StopCoroutine(Roam());
    }

    //Has Monster stand idly. Used in inactive state and when noticing the player
    private IEnumerator StandIdle(float timeToStand){
        //Debug.Log("Idle");
        standingIdle = true;
        pausingIEnumerator = false;

        pauseStandIdle = StartCoroutine(Pause(timeToStand));
        navMeshAgent.SetDestination(transform.position);

        while(((!IsPlayerVisible() || !canSeePlayer) || currentState == (int) monsterState.chasingPlayer) && pausingIEnumerator){
            yield return null;
            if(currentState == (int) monsterState.chasingPlayer) cameraHolder.LookAt(head.position);
            head.LookAt(playerGO.transform.position);
        }

        if(currentState == (int) monsterState.chasingPlayer) { 
            chasing = true;
            playerCameraController.canLookAround = true;
            playerMovement.canMove = true;
        }

        if((IsPlayerVisible() && canSeePlayer) && currentState != (int) monsterState.chasingPlayer) {
            noticingPlayer = true;
            currentState = (int) monsterState.chasingPlayer;
            //Debug.Log("chasing");
            standingIdle = false;
            playerCameraController.canLookAround = false;
            playerMovement.canMove = false;
            StopCoroutine(StandIdle(timeToStand));
        }

        if(currentState != (int) monsterState.chasingPlayer){
            currentState = (int) monsterState.roaming;
            //Debug.Log("roaming");
        }

        standingIdle = false;
        StopCoroutine(StandIdle(timeToStand));
    }

    private IEnumerator Pause(float timeToWait) {
        pausingIEnumerator = true;
        //Debug.Log("Pausing");
        yield return new WaitForSeconds(timeToWait);
        //Debug.Log("Done Pausing");
        pausingIEnumerator = false;
        StopCoroutine(Pause(timeToWait));
    }
}
