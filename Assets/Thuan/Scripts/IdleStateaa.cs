using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStatete : StateMachineBehaviour
{
    float timer;
    Transform player;
    float chaseRange = 8;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer > 5)
            animator.SetBool("isPatrolling", true);

        if (player != null)  // Kiểm tra player trước khi truy cập
        {
            float distance = Vector3.Distance(player.position, animator.transform.position);
            if (distance < chaseRange)
            {
                animator.SetTrigger("growl");
                animator.SetBool("isPatrolling", false);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("growl");
    }
}
