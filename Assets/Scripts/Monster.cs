using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private GameObject playerGO;
    [SerializeField]
    private LayerMask playerLayer;

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
        if(currentState == (int) monsterState.inactive){
            if(IsPlayerVisible()) currentState = (int) monsterState.chasingPlayer;
            Debug.Log("Chase");
        }
    }

    private bool IsPlayerVisible (){
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(transform.position, playerGO.transform.position-transform.position);
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        Debug.Log("Ray Hit: " + hit.transform.name);
        if(hit.collider.gameObject.layer == playerLayer) return true;
        
        return false;
    } 
}
