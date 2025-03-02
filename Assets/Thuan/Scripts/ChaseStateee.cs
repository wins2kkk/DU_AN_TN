using UnityEngine;
using UnityEngine.AI;

public class ChaseStatete : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    private float chaseSpeed = 4.5f;
    private float stopChaseDistance = 15f; // Mất dấu nếu Player cách xa hơn 15m
    private float attackRange = 2f; // Bắt đầu tấn công khi gần Player

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null && agent.enabled)
        {
            agent.speed = chaseSpeed;   
            agent.isStopped = false;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || agent == null || !agent.enabled) return;

        float distance = Vector3.Distance(player.position, animator.transform.position);

        if (distance > stopChaseDistance)
        {
            // Nếu mất dấu, quay về tuần tra
           // animator.SetTrigger("isGu");
            return;
        }

        if (distance < attackRange)
        {
            // Khi đến gần Player, chuyển sang tấn công
            animator.SetBool("isAttacking", true);
        }
        else
        {
            // Nếu chưa tới gần, tiếp tục đuổi theo
            agent.SetDestination(player.position);
            animator.SetBool("isAttacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null && agent.enabled)
        {
            agent.SetDestination(animator.transform.position);
        }
    }
}
