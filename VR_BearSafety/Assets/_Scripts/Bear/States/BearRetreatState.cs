using UnityEngine;
using UnityEngine.AI;

public class BearRetreatState : BearMovementBaseState
{
    private float _fleeTimer;

    private float _fleeDistance;
    private float _fleeTargetInterval;
    private float _fleeRandomAngle;
    private float _fleeNavMeshSampleDistance;

    private float _youSurvivedMenuWait = 3.0f;


    public BearRetreatState(Animator animator, Camera playerRef, NavMeshAgent bearAgent, float newAgentSpeed,
                float fleeDistance, float fleeTargetInterval, float fleeRandomAngle, float fleeNavMeshSampleDistance
        ) : base(animator, playerRef, bearAgent, newAgentSpeed)
    {
        _fleeDistance = fleeDistance;
        _fleeTargetInterval = fleeTargetInterval;
        _fleeRandomAngle = fleeRandomAngle;
        _fleeNavMeshSampleDistance = fleeNavMeshSampleDistance;
    }


    public override void Enter()
    {
        _bearAgent.isStopped = false;
        _bearAgent.speed = _agentSpeed;

        _fleeTimer = 0f;

        animator.CrossFade("Retreat", 0.2f);
    }

    public override void Update()
    {
        _fleeTimer -= Time.deltaTime;

        if (_fleeTimer <= 0f)
        {
            _fleeTimer = _fleeTargetInterval;

            if (TryGetEscapePosition(out Vector3 target))
            {
                _bearAgent.SetDestination(target);
            }
        }

        _youSurvivedMenuWait -= Time.deltaTime;

        if (_youSurvivedMenuWait <= 0.0f && _youSurvivedMenuWait >= -10.0f)
        {
            TutorialEvent.ReportAction(TutorialStep.BearSurvived);
            _youSurvivedMenuWait = -1000.0f;
        }
    }


    private bool TryGetEscapePosition(out Vector3 result)
    {
        result = Vector3.zero;

        Vector3 awayDirection = (_bearAgent.transform.position - _playerRef.transform.position).normalized;
        awayDirection.y = 0f;
        awayDirection.Normalize();

        for (int i = 0; i < 10; i++)
        {
            float randomAngle = Random.Range(-_fleeRandomAngle, _fleeRandomAngle);

            Vector3 direction = Quaternion.Euler(0f, randomAngle, 0f) * awayDirection;

            Vector3 candidate = _bearAgent.transform.position + direction * _fleeDistance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, _fleeNavMeshSampleDistance, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        return false;
    }
}
