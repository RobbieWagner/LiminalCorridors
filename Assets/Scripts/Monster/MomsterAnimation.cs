using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomsterAnimation : MonoBehaviour
{

    [SerializeField] private Monster monster;
    [SerializeField] private Animator monsterAnimator;

    private bool roaming;
    private bool chasing;
    // Start is called before the first frame update
    void Start()
    {
        monsterAnimator.SetBool("roaming", false);
        monsterAnimator.SetBool("chasing", false);

        roaming = false;
        chasing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(monster.currentState == (int) Monster.monsterState.chasingPlayer 
        || monster.currentState == (int) Monster.monsterState.lookingForPlayer) {
            chasing = true;
            roaming = false;
        }
        else if(monster.currentState == (int) Monster.monsterState.roaming){ roaming = true;
            chasing = false;
            roaming = true;
        }
        else {
            chasing = false;
            roaming = false;
        }

        monsterAnimator.SetBool("roaming", roaming);
        monsterAnimator.SetBool("chasing", chasing);
    }
}
