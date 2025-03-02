using Akila.FPSFramework;
using UnityEngine;
using System.Collections;

public class AttackDamage : StateMachineBehaviour
{
    public float attackDamage = 35f;
    private Transform player;
    private Coroutine attackCoroutine;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            attackCoroutine = animator.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(DelayedAttack(animator));
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Nếu Animation bị hủy giữa chừng, dừng Coroutine để không gây sát thương
        if (attackCoroutine != null)
        {
            animator.gameObject.GetComponent<MonoBehaviour>().StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private IEnumerator DelayedAttack(Animator animator)
    {
        yield return new WaitForSeconds(1.5f);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            if (player != null)
            {
                HealthSystem playerHealth = player.GetComponent<HealthSystem>();

                if (playerHealth != null)
                {
                    playerHealth.Damage(attackDamage, null);

                    if (playerHealth.GetHealth() <= 0)
                    {
                      
                    }
                }
            }
        }
    }



}


