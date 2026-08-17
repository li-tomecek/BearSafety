using UnityEngine;
using UnityEngine.AI;

public class BearAttackState : BaseState
{
    private NavMeshAgent _bearAgent;


    public BearAttackState(Animator animator, NavMeshAgent bearAgent) : base(animator)
    {
        _bearAgent = bearAgent;
    }


    public override void Enter()
    {
        _bearAgent.isStopped = true;

        _bearAgent.velocity = Vector3.zero;
        _bearAgent.ResetPath();

        animator.CrossFade("Attack", 0.2f);
    }
}
