using UnityEngine;
using UnityEngine.AI;

public class BearWalkingState : BearMovementBaseState
{
    private BearController _bearController;

    private float _walkDuration;
    private float _walkDurationRemaining;


    public BearWalkingState(Animator animator, BearController bearController, float walkDuration, Camera playerRef, NavMeshAgent bearAgent, float newAgentSpeed) : base(animator, playerRef, bearAgent, newAgentSpeed)
    {
        _bearController = bearController;
        _walkDuration = walkDuration;
    }


    public override void Enter()
    {
        base.Enter();

        animator.CrossFade("Walk", 0.2f);

        _walkDurationRemaining = _walkDuration;
    }

    public override void Update()
    {
        base.Update();

        _walkDurationRemaining -= Time.deltaTime;

        if (_walkDurationRemaining <= 0.0f)
            _bearController.TransitionToStalk();
    }
}
