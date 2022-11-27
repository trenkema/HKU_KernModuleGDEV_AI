using UnityEngine;
using BehaviourTree;

public enum AnimationTypes
{
    Trigger,
    Bool,
    Float,
    Int
}

public class BTPlayAnimation : Node
{
    private Animator animator;
    private string animation;
    private bool animationState = false;
    private string animationValue;
    private AnimationTypes animationType;

    public BTPlayAnimation(Animator _animator, string _animation, bool _animationState, string _animationValue, AnimationTypes _animationType)
    {
        animator = _animator;
        animation = _animation;
        animationState = _animationState;
        animationValue = _animationValue;
        animationType = _animationType;
    }

    public override NodeState Evaluate()
    {
        switch (animationType)
        {
            case AnimationTypes.Trigger:
                animator.SetTrigger(animation);
                break;
            case AnimationTypes.Bool:
                animator.SetBool(animation, animationState);
                break;
            case AnimationTypes.Float:
                animator.SetFloat(animation, float.Parse(animationValue));
                break;
            case AnimationTypes.Int:
                animator.SetInteger(animation, int.Parse(animationValue));
                break;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}
