
using UnityEngine;
using UnityEngine.AI;

public class BearStalkState : BearMovementBaseState
{
    private BearController _bearController;

    private float _stalkingTimer;

    private float _stalkingMinDistance;
    private float _stalkingMaxDistance;
    private float _stalkingTargetInterval;
    private float _stalkingNavMeshSampleDistance;

    private int _timesBeforeSwap;


    public BearStalkState(Animator animator, BearController bearController, CharacterController playerRef, NavMeshAgent bearAgent, float newAgentSpeed,
                float stalkingMinDistance, float stalkingMaxDistance, float stalkingTargetInterval, float stalkingNavMeshSampleDistance
        ) : base(animator, playerRef, bearAgent, newAgentSpeed)
    {
        _bearController = bearController;

        _stalkingMinDistance = stalkingMinDistance;
        _stalkingMaxDistance = stalkingMaxDistance;
        _stalkingTargetInterval = stalkingTargetInterval;
        _stalkingNavMeshSampleDistance = stalkingNavMeshSampleDistance;
    }


    public override void Enter()
    {
        _bearAgent.isStopped = false;
        _bearAgent.speed = _agentSpeed;

        animator.CrossFade("Stalk", 0.2f);

        _stalkingTimer = 0.0f;

        _timesBeforeSwap = 4;
    }

    public override void Update()
    {
        _stalkingTimer -= Time.deltaTime;

        if (_stalkingTimer <= 0f)
        {
            _timesBeforeSwap--;

            if (_timesBeforeSwap <= 0)
            {
                if (_bearController.CanSeePlayer)
                    _bearController.TransitionToWalk();
                else
                    _bearController.TransitionToIdle();
                    
                return;
            }


            _stalkingTimer = _stalkingTargetInterval;

            if (TryGetStalkingPosition(out Vector3 target))
            {
                _bearAgent.SetDestination(target);
            }
            else
            {
                _bearController.TransitionToIdle();
            }
        }
    }

    private bool TryGetStalkingPosition(out Vector3 result)
    {
        result = Vector3.zero;

        for (int i = 0; i < 10; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            float distance = Random.Range(_stalkingMinDistance, _stalkingMaxDistance);

            Vector3 candidate = _playerRef.transform.position + new Vector3(randomDirection.x, 0f, randomDirection.y) * distance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, _stalkingNavMeshSampleDistance, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        return false;
    }
}
