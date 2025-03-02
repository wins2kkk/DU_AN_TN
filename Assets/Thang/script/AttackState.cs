using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    Transform player;
    //PlayerHealth playerHealth; // Thêm tham chiếu đến hệ thống sức khỏe của nhân vật
    float attackCooldown = 1.7f; // Thời gian giữa các đòn tấn công
    float lastAttackTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
        {
            //playerHealth = player.GetComponent<PlayerHealth>();
        }

        lastAttackTime = Time.time; // Khởi tạo thời gian tấn công lần đầu
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (player == null || playerHealth == null) return;

        // Enemy nhìn về phía player
        animator.transform.LookAt(player);

        // Kiểm tra khoảng cách và tắt trạng thái tấn công nếu khoảng cách quá xa
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > 3.5f)
        {
            animator.SetBool("isAttacking", false);
            return;
        }

        // Nếu đủ thời gian giữa các đòn tấn công
        if (Time.time >= lastAttackTime + attackCooldown)
        {
           // FindObjectOfType<AudioManager>().Play("Damplayer");
            // Gây sát thương cho player
            //playerHealth.TakeDamage(20);
            lastAttackTime = Time.time;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Có thể thêm logic khi rời khỏi trạng thái attack (nếu cần)
    }
}
