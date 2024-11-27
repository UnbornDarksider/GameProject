using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateScript : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.2f || Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K))
        {
            animator.SetBool("isFast", true);
        }
        else
        {
            animator.SetBool("isFast", false);
        }
    }
}
