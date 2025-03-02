using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZOMBIEA
{
    public int HP = 10;
    public Animator animator;
    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        if (HP <= 0)
        {
            animator.SetTrigger("die");
        }
        else
        {
            animator.SetTrigger("damage");
        }
    }
}
