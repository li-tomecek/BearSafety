
using UnityEngine;
using UnityEngine.AI;

public class BearChaseState : BearMovementBaseState
{
    private BearController _bearController;
    private float _attackRange;


    public BearChaseState(Animator animator, BearController bearController, float attackRange,
        Camera playerRef, NavMeshAgent bearAgent, float newAgentSpeed) : base(animator, playerRef, bearAgent, newAgentSpeed)
    {
        _bearController = bearController;
        _attackRange = attackRange;
    }


    public override void Enter()
    {
        base.Enter();

        animator.CrossFade("Chase", 0.2f);
    }

    public override void Update()
    {
        base.Update();

        if (Vector3.Distance(_bearAgent.transform.position, _playerRef.transform.position) <= _attackRange)
        {
            _bearController.TransitionToAttack();
        }
    }
}
