using UnityEngine;
using UnityEngine.AI;

public class BearMovementBaseState : BaseState
{
    protected Camera _playerRef;
    protected NavMeshAgent _bearAgent;
    protected float _agentSpeed;


    public BearMovementBaseState(Animator animator, Camera playerRef, NavMeshAgent bearAgent, float newAgentSpeed) : base(animator)
    {
        _playerRef = playerRef;
        _bearAgent = bearAgent;
        _agentSpeed = newAgentSpeed;
    }


    public override void Enter()
    {
        _bearAgent.speed = _agentSpeed;
        _bearAgent.isStopped = false;
    }

    public override void Update()
    {
        _bearAgent.SetDestination(_playerRef.transform.position);
    }
}
