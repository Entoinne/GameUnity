using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyBehavior : MonoBehaviour
{

    public Animator animator;
    public float hp;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("isFighting", true);
        hp = 20;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            animator.SetBool("isDead", true);
        }
    }
}
