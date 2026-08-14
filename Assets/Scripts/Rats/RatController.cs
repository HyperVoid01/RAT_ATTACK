using UnityEngine;
using UnityEngine.AI;

public class RatController : MonoBehaviour, ITargetable
{
    [SerializeField] private RatData data;
    
    private int currentHealth;
    public Transform player;
    private NavMeshAgent agent;
    private float idleTimer;
    private bool isWaiting;
    private bool isFleeing;
    private ITargetable targetableImplementation;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = data.speed;
    }

    private void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Flees when player is nearby
        if (distToPlayer <= data.fleeRadius)
        {
            isFleeing = true;
            isWaiting = false;
            Flee();
            return;
        }

        // Stops fleeing when player is far away
        if (isFleeing)
        {
            isFleeing = false;
            SetNewDestination();
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                idleTimer = Random.Range(data.minIdleTime, data.maxIdleTime);
            }
            else
            {
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0f)
                {
                    isWaiting = false;
                    SetNewDestination();
                }
            }
        }
    }

    private void Flee()
    {
        Vector3 fleeTarget = GetFleeLocation();
        agent.SetDestination(fleeTarget);
    }

    private Vector3 GetFleeLocation()
    {
        // Direction from player to rat, extended out to fleeDistance
        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;
        Vector3 candidatePoint = transform.position + directionAwayFromPlayer * data.fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(candidatePoint, out hit, data.fleeDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // Fallback if that direction is off the mesh 
        return GetRandomLocation();
    }

    private void SetNewDestination()
    {
        Vector3 randomPoint = GetRandomLocation();
        agent.SetDestination(randomPoint);
    }

    private Vector3 GetRandomLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * data.wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, data.wanderRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.fleeRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, data.wanderRadius);
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, data.maxHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Heal(int healing)
    {
        currentHealth = Mathf.Clamp(currentHealth + healing, 0, data.maxHealth);
    }
}
