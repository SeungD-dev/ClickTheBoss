using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    private int currentAttack = 0;
    private float timeSinceAttack = 0.0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        timeSinceAttack += Time.deltaTime;
    }

    public void BossClicked()
    {
        if(timeSinceAttack > 0.25f)
        {
            currentAttack++;
            
            if (currentAttack > 3)
                currentAttack = 1;

            
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;

            
            animator.SetTrigger("Attack" + currentAttack);

            
            timeSinceAttack = 0.0f;
        }
    }
}
