using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorLocalHandler : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
    }

    public void ActiveAnimatonLocal() {
        animator.SetBool("isRunAnimationLocal", true);
    }

    public void DeActiveAnimatonLocal() {
        animator.SetBool("isRunAnimationLocal", false);
    }
}
