using Akila.FPSFramework;
using UnityEngine;
using UnityEngine.AI;

public class chaseState : StateMachineBehaviour
{
    NavMeshAgent agent;
    Transform player;
    float attackTimer = 0f;
    bool explosionTriggered = false;
    zombieAAA zombieScript;

    // OnStateEnter: Được gọi khi state bắt đầu
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Lấy NavMeshAgent và kiểm tra null
        agent = animator.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("Không tìm thấy NavMeshAgent trên đối tượng: " + animator.gameObject.name);
            return;
        }

        // Tìm player và kiểm tra null
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Player với tag 'Player'!");
            return;
        }

        // Đặt tốc độ cho agent
        agent.speed = 4.5f;

        // Reset timer và trạng thái
        attackTimer = 0f;
        explosionTriggered = false;

        // Lấy component zombieAAA và kiểm tra null
        zombieScript = animator.GetComponent<zombieAAA>();
        if (zombieScript == null)
        {
            Debug.LogWarning("Không tìm thấy component zombieAAA trên đối tượng: " + animator.gameObject.name);
        }
    }

    // OnStateUpdate: Được gọi mỗi frame khi state đang hoạt động
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Kiểm tra null trước khi tiếp tục
        if (agent == null || player == null)
        {
            Debug.LogWarning("Agent hoặc Player là null, không thể tiếp tục chase!");
            animator.SetBool("isChasing", false); // Thoát trạng thái chase nếu có lỗi
            return;
        }

        // Đặt đích cho agent
        agent.SetDestination(player.position);

        // Tính khoảng cách giữa enemy và player
        float distance = Vector3.Distance(player.position, animator.transform.position);

        // Nếu player quá xa, tắt trạng thái chase và reset timer
        if (distance > 15f)
        {
            animator.SetBool("isChasing", false);
            attackTimer = 0f;
            return;
        }

        // Nếu player ở trong vùng tấn công (< 2.5 đơn vị)
        if (distance < 2.5f)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= 0.1f && !explosionTriggered)
            {
                explosionTriggered = true;
                if (zombieScript != null)
                {
                    zombieScript.TriggerExplosionNow();
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy component zombieAAA để kích hoạt nổ!");
                }
            }
        }
        else
        {
            // Reset timer nếu player ra khỏi vùng tấn công
            attackTimer = 0f;
        }
    }

    // OnStateExit: Được gọi khi state kết thúc
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent != null)
        {
            agent.SetDestination(animator.transform.position); // Dừng agent tại vị trí hiện tại
        }
    }
}