using UnityEngine;

public class AttackStatete : StateMachineBehaviour
{
    private Transform player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) return;

        // Xoay Enemy hướng về Player nhưng chỉ trên trục Y
        Vector3 direction = player.position - animator.transform.position;
        direction.y = 0; // Giữ nguyên trục Y
        animator.transform.rotation = Quaternion.LookRotation(direction);

        // Kiểm tra khoảng cách để thoát trạng thái Attack
        float distance = Vector3.Distance(player.position, animator.transform.position);
        if (distance > 3f)
        {
            animator.SetBool("isAttacking", false);
        }
    }
}
