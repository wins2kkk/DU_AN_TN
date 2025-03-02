using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine;

public class WalkStatee : StateMachineBehaviour
{
    float timer;
    List<Transform> wayPoints = new List<Transform>();
    NavMeshAgent agent;

    Transform player;
    float chaseRange = 8;
    bool isGrowling = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 2;
        timer = 0;

        GameObject go = GameObject.FindGameObjectWithTag("WayPoints");
        foreach (Transform t in go.transform)
            wayPoints.Add(t);

        agent.SetDestination(wayPoints[Random.Range(0, wayPoints.Count)].position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, animator.transform.position);

            if (distance < chaseRange)
            {
                if (!isGrowling)
                {
                    animator.SetTrigger("growl");
                    isGrowling = true;
                }
            }
            else
            {
                if (isGrowling)
                {
                    animator.ResetTrigger("growl");
                    isGrowling = false;
                }

                // Tuần tra nếu còn điểm tuần tra
                if (wayPoints.Count > 0 && agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.SetDestination(wayPoints[Random.Range(0, wayPoints.Count)].position);
                }

                timer += Time.deltaTime;
                if (timer > 10)
                    animator.SetBool("isPatrolling", false);
            }
        }
        else
        {
            Debug.LogWarning("Player không tồn tại.");
        }
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
        animator.ResetTrigger("growl"); // Đảm bảo trigger growl được reset khi rời khỏi trạng thái
        isGrowling = false; // Đảm bảo trạng thái growling được reset
    }
}
