using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GrowlState : StateMachineBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private List<Transform> wayPoints = new List<Transform>();

    private float patrolTimer;
    private float patrolDuration = 10f; // Thời gian tuần tra trước khi dừng
    private float chaseRange = 10f; // Khoảng cách phát hiện Player
    private float stopChaseDistance = 15f; // Khoảng cách mất mục tiêu

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = animator.GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.speed = 1.5f;
        patrolTimer = 0;

        // Lấy danh sách WayPoints
        GameObject go = GameObject.FindGameObjectWithTag("WayPoints");
        if (go != null)
        {
            foreach (Transform t in go.transform)
            {
                wayPoints.Add(t);
            }
        }
        else
        {
            Debug.LogError("WayPoints object not found!");
        }

        // Đặt điểm đến nếu có waypoint hợp lệ
        if (wayPoints.Count > 0)
        {
            SetRandomWaypoint();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player != null)
        {
            float distance = Vector3.Distance(player.position, animator.transform.position);

            if (distance < chaseRange)
            {
                // Chuyển sang trạng thái đuổi theo
                agent.speed = 3.5f;
                agent.SetDestination(player.position);
                patrolTimer = 0; // Reset thời gian tuần tra
              //  Debug.Log("Chasing Player!");
                animator.SetBool("isGu", true);

            }
            else if (distance > stopChaseDistance)
            {
                // Nếu mất mục tiêu, quay lại tuần tra
                agent.speed = 1.5f;
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    SetRandomWaypoint();
                }

                patrolTimer += Time.deltaTime;
                if (patrolTimer > patrolDuration)
                {
                   // Debug.Log("Patrol ended.");
                    animator.SetBool("isPatrolling", true);
                }
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position); // Dừng AI khi chuyển trạng thái
    }

    private void SetRandomWaypoint()
    {
        if (wayPoints.Count > 0)
        {
            Transform targetWaypoint = wayPoints[Random.Range(0, wayPoints.Count)];
            agent.SetDestination(targetWaypoint.position);
            Debug.Log("Moving to Waypoint: " + targetWaypoint.position);
        }
    }
}
