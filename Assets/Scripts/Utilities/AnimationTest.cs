using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AnimationTest : MonoBehaviour
{
    [SerializeField] private AnimationClip _clip;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        AnimatorOverrideController aoc = new AnimatorOverrideController(_animator.runtimeAnimatorController);

        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, _clip));

        aoc.ApplyOverrides(anims);

        _animator.runtimeAnimatorController = aoc;
    }

    [Button(ButtonSizes.Large)]
    private void PlayAnimation()
    {
        _animator.Play("1");
    }
}
