using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Wandering Settings")]
    public float wanderRadius = 20f;
    public float wanderDelay = 3f;
    public float waitTimeAtPoint = 2f;

    [Header("Detection Settings")]
    public float detectionRadius = 15f;
    public LayerMask detectionLayers;
    public string playerTag = "Player";

    [Header("Combat Settings")]
    public float attackRange = 10f;
    public float fireRate = 1f;
    public Transform gunMuzzle;
    public GameObject bulletPrefab;

    [Header("Death Reporting")]
    public EnemySpawner spawner;

    [Header("Gizmos Settings")]
    public bool showGizmos = true;
    public Color wanderRadiusColor = Color.yellow;
    public Color detectionRadiusColor = Color.red;
    public Color attackRangeColor = Color.magenta;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentTarget;
    private Vector3 wanderPoint;
    private bool isWandering = true;
    private bool isAttacking = false;
    private float lastFireTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        StartCoroutine(WanderRoutine());
    }

    void Update()
    {
        UpdateAnimations();
        DetectPlayer();

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget <= attackRange)
            {
                agent.isStopped = true;
                isAttacking = true;
                FaceTarget();

                if (Time.time >= lastFireTime + fireRate)
                {
                    Fire();
                    lastFireTime = Time.time;
                }
            }
            else
            {
                agent.isStopped = false;
                isAttacking = false;
                agent.SetDestination(currentTarget.position);
            }
        }
        else if (isWandering)
        {
            agent.isStopped = false;
            isAttacking = false;
        }
    }

    IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (isWandering && currentTarget == null)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
                if (FindValidNavMeshPoint(randomPoint, out wanderPoint))
                {
                    agent.SetDestination(wanderPoint);
                }
            }
            yield return new WaitForSeconds(wanderDelay);
        }
    }

    bool FindValidNavMeshPoint(Vector3 center, out Vector3 result)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(center, out hit, wanderRadius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayers);
        currentTarget = null;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(playerTag))
            {
                currentTarget = hit.transform;
                isWandering = false;
                return;
            }
        }

        if (currentTarget == null)
        {
            isWandering = true;
        }
    }

    void FaceTarget()
    {
        if (currentTarget == null) return;
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Fire()
    {
        if (bulletPrefab != null && gunMuzzle != null)
        {
            Instantiate(bulletPrefab, gunMuzzle.position, gunMuzzle.rotation);
        }
    }

    void UpdateAnimations()
    {
        animator.SetBool("Run", agent.velocity.magnitude > 0.1f && !isAttacking);
        animator.SetBool("Shoot", isAttacking);
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Draw wander radius
        Gizmos.color = wanderRadiusColor;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        // Draw detection radius
        Gizmos.color = detectionRadiusColor;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw attack range
        Gizmos.color = attackRangeColor;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw current wander path
        if (isWandering)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, wanderPoint);
            Gizmos.DrawSphere(wanderPoint, 0.5f);
        }
    }
}