using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class BearController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 300.0f;
    private float _currentHealth = 0.0f;
    public float MaxHealth { get; private set; }

    [Header("SpawnMode")]
    [SerializeField] private BearSpawnMode spawnMode = BearSpawnMode.RandomAroundPlayer;

    [Header("SpawnMode - RandomAroundPlayer")]
    [SerializeField] private CharacterController playerRef;
    [SerializeField] private float minSpawnDistance = 1000.0f;
    [SerializeField] private float maxSpawnDistance = 2000.0f;


    [Header("SpawnMode - SpawnPoints")]
    [SerializeField] private Transform[] spawnPoints;


    [Header("States - Idle")]
    [SerializeField] private float minIdleTime = 2.0f;
    [SerializeField] private float maxIdleTime = 5.0f;

    [Header("States - Walk")]
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float walkDuration = 3.0f;

    [Header("States - Chase")]
    [SerializeField] private float chaseSpeed = 50.0f;
    [SerializeField] private float chaseRange = 15.0f;

    [Header("States - Attack")]
    [SerializeField] private float attackRange = 15.0f;

    [Header("States - Stalk")]
    [SerializeField] private float stalkSpeed = 5.0f;
    [SerializeField] private float stalkingMinDistance = 5.0f;
    [SerializeField] private float stalkingMaxDistance = 15.0f;
    [SerializeField] private float stalkingTargetInterval = 3.0f;
    [SerializeField] private float stalkingNavMeshSampleDistance = 5.0f;

    [Header("States - Retreat")]
    [SerializeField] private float fleeSpeed = 50.0f;
    [SerializeField] private float fleeDistance = 20.0f;
    [SerializeField] private float fleeTargetInterval = 1.0f;
    [SerializeField] private float fleeRandomAngle = 45.0f;
    [SerializeField] private float fleeNavMeshSampleDistance = 10.0f;

    [Header("LoS")]
    [SerializeField] private float detectionRange = 25.0f;
    [SerializeField] private LayerMask lineOfSightMask;
    public bool CanSeePlayer { get; private set; } = false;


    private Animator _animator;

    private NavMeshAgent _agent;

    private StateMachine<IState> _stateMachine;

    private bool _isPaused = false;


    private void Awake()
    {
        MaxHealth = maxHealth;

        _animator = GetComponent<Animator>();

        _agent = GetComponent<NavMeshAgent>();

        _stateMachine = new StateMachine<IState>(new BearIdleState(_animator, this, _agent, minIdleTime, maxIdleTime));

        _stateMachine.AddState(new BearWalkingState(_animator, this, walkDuration, playerRef, _agent, walkSpeed));
        _stateMachine.AddState(new BearChaseState(_animator, this, attackRange, playerRef, _agent, chaseSpeed));
        _stateMachine.AddState(new BearStalkState(_animator, this, playerRef, _agent, stalkSpeed, stalkingMinDistance, stalkingMaxDistance, stalkingTargetInterval, stalkingNavMeshSampleDistance));
        _stateMachine.AddState(new BearRetreatState(_animator, playerRef, _agent, fleeSpeed, fleeDistance, fleeTargetInterval, fleeRandomAngle, fleeNavMeshSampleDistance));
        _stateMachine.AddState(new BearAttackState(_animator, _agent));
    }

    private void Start()
    {
        Spawn();
    }

    private void Update()
    {
        if (_isPaused)
        {
            TransitionToIdle();
            return;
        }

        CanSeePlayer = CheckCanSeePlayer();


        if (IsPlayerWithinChargeRange() && HasPathToPlayer())
        {
            if (_stateMachine.TryGetState(out BearAttackState attackState) && _stateMachine.TryGetState(out BearRetreatState retreatState))
            {
                if (_stateMachine.CurrentState != retreatState)
                    if (_stateMachine.CurrentState != attackState)
                        TransitionToChase();
            }
        }


        _stateMachine?.Update();
    }


    private bool IsPlayerWithinChargeRange()
    {
        float distance = Vector3.Distance(transform.position, playerRef.transform.position);

        return distance <= chaseRange;
    }

    private bool HasPathToPlayer()
    {
        NavMeshPath path = new NavMeshPath();

        if (!_agent.CalculatePath(playerRef.transform.position, path))
            return false;

        return path.status == NavMeshPathStatus.PathComplete;
    }


    private bool CheckCanSeePlayer()
    {
        Vector3 startPosition = transform.position + Vector3.up;
        Vector3 direction = playerRef.transform.position - startPosition;
        direction.y += 1.0f;
        
        float distance = direction.magnitude;

        if (distance > detectionRange)
            return false;

        direction.Normalize();

        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, detectionRange, lineOfSightMask))
        {
            if (hit.transform.GetComponentInParent<CharacterController>())
            {
                return true;
            }
        }

        return false;
    }


    public void PauseBear() { _isPaused = true; }
    public void UnPauseBear() { _stateMachine.ReturnToPreviousState(); _isPaused = false; }


    public void Spawn()
    {
        Vector3 spawnPosition = Vector3.zero;

        if (spawnMode == BearSpawnMode.RandomAroundPlayer)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

            spawnPosition = playerRef.transform.position + new Vector3(randomDirection.x, 0f, randomDirection.y) * distance;
        }
        else if (spawnMode == BearSpawnMode.SpawnPointList)
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning("No bear spawn points assigned.");
                return;
            }

            spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }

        transform.position = spawnPosition;


        _currentHealth = maxHealth;


        gameObject.SetActive(true);
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0.0f)
        {
            TransitionToRetreat();
        }
    }

    public void TransitionToIdle() => _stateMachine.TransitionTo<BearIdleState>();
    public void TransitionToWalk() => _stateMachine.TransitionTo<BearWalkingState>();
    public void TransitionToChase() => _stateMachine.TransitionTo<BearChaseState>();
    public void TransitionToAttack() => _stateMachine.TransitionTo<BearAttackState>();
    public void TransitionToStalk() => _stateMachine.TransitionTo<BearStalkState>();
    public void TransitionToRetreat() => _stateMachine.TransitionTo<BearRetreatState>();
}