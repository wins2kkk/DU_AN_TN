using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class patrollState : StateMachineBehaviour
{
    float timer;
    List<Transform> wayPoints = new List<Transform>();
    NavMeshAgent agent;
    Transform player;
    //Pham vi 10 thi duoi theo
    float chaseRange = 10;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Kiểm tra null
        if (player == null)
        {
            Debug.LogError("Không tìm thấy GameObject với tag 'Player'!");
            return;
        }

        agent = animator.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent không tồn tại trên GameObject!");
            return;
        }

        // Kiểm tra xem agent có hoạt động và nằm trên NavMesh không
        if (!agent.gameObject.activeInHierarchy || !agent.isOnNavMesh)
        {
            //Debug.LogError("Agent không hoạt động hoặc không nằm trên NavMesh!");
            return;
        }

        agent.speed = 1f;
        timer = 0;

        GameObject go = GameObject.FindGameObjectWithTag("WayPoints");
        if (go == null)
        {
            Debug.LogError("Không tìm thấy GameObject với tag 'WayPoints'!");
            return;
        }

        foreach (Transform t in go.transform)
            wayPoints.Add(t);

        // Kiểm tra xem waypoint có hợp lệ và nằm trên NavMesh không
        Transform randomWaypoint = wayPoints[Random.Range(0, wayPoints.Count)];
        if (NavMesh.SamplePosition(randomWaypoint.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            // Đảm bảo agent có thể di chuyển đến vị trí này
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(hit.position);
            }
            else
            {
                //Debug.LogError("Agent không nằm trên NavMesh, không thể đặt đích đến!");
            }
        }
        else
        {
            //Debug.LogError("Waypoint không nằm trên NavMesh!");
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent == null || !agent.isOnNavMesh || !agent.gameObject.activeInHierarchy)
        {
            //Debug.Log("PHÁT HIỆN PLAYER!");
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance && agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            Transform randomWaypoint = wayPoints[Random.Range(0, wayPoints.Count)];
            if (NavMesh.SamplePosition(randomWaypoint.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        timer += Time.deltaTime;
        if (timer > 10)
            animator.SetBool("isPatrolling", false);

        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance < chaseRange)
            animator.SetBool("isChasing", true);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
