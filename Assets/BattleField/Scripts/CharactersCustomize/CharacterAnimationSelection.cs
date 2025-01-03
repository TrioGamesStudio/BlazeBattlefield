using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationSelection : MonoBehaviour
{
    private Animator animator;
    public List<int> animationsHash;
    private void Awake()
    {
        animationsHash = new();
        animator = GetComponent<Animator>();
        
        foreach (var animationClip in 
            animator.runtimeAnimatorController.animationClips)
        {
            Debug.Log("animation name: " + animationClip.name);
            animationsHash.Add(Animator.StringToHash(animationClip.name));
        }
    }
    public int index;
    [Button]
    public void Test()
    {
        animator.Play(animationsHash[index]);
    }
}
