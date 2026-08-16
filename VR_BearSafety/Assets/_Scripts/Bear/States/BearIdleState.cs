
using UnityEngine;
using UnityEngine.AI;

public class BearIdleState : BaseState
{
    private BearController _bearController;

    private NavMeshAgent _bearAgent;

    private float _minIdleTime;
    private float _maxIdleTime
        ;
    private float _idleTimer;


    public BearIdleState(Animator animator, BearController bearController, NavMeshAgent bearAgent, float minIdleTime, float maxIdleTime) : base(animator)
    {
        _bearController = bearController;

        _bearAgent = bearAgent;

        _minIdleTime = minIdleTime;
        _maxIdleTime = maxIdleTime;
    }


    public override void Enter()
    {
        _bearAgent.isStopped = true;

        animator.CrossFade("Idle", 0.2f);

        _idleTimer = Random.Range(_minIdleTime, _maxIdleTime);
    }

    public override void Update()
    {
        _idleTimer -= Time.deltaTime;

        if (_idleTimer <= 0.0f)
        {
            _bearController.TransitionToStalk();
        }
    }
}
