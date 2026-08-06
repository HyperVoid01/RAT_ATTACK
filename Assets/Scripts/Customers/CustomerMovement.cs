using UnityEngine;
using UnityEngine.AI;

public class CustomerMovement : MonoBehaviour
{
    [SerializeField] private CustomerData data;
    [SerializeField] private Transform exitPoint; // where customers walk to after being served

    private bool slotReserved;
    private int queueSlot;
    private int lastSeenQueueVersion;
    private NavMeshAgent agent;
    private CustomerBehaviour behaviour;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        behaviour = GetComponent<CustomerBehaviour>();
    }

    private void Start()
    {
        TryReserveSlot();
    }

    private void Update()
    {
        // Line is physically full - keep polling for a free slot instead
        // of indexing into lineSlots with an invalid position.
        if (!slotReserved)
        {
            TryReserveSlot();
            return;
        }

        // Catch up on ALL missed shifts at once, even if this customer is
        // still walking (not yet "inLine") or several serves happened
        // while they were en route. Using a delta instead of a boolean
        // check means no shift ever gets silently dropped.
        int versionDelta = OrderStation.Instance.QueueVersion - lastSeenQueueVersion;
        if (versionDelta > 0 && queueSlot > 0)
        {
            int newSlot = Mathf.Max(0, queueSlot - versionDelta);
            if (newSlot != queueSlot)
            {
                queueSlot = newSlot;
                behaviour.inLine = false;
                agent.SetDestination(OrderStation.Instance.lineSlots[queueSlot].transform.position);
            }

            lastSeenQueueVersion = OrderStation.Instance.QueueVersion;
        }

        if (!behaviour.inLine && Vector3.Distance(transform.position, agent.destination) < 0.5f)
        {
            behaviour.inLine = true;

            // Only register for service once - shifting forward later must not re-add customer.
            if (!behaviour.joinedQueue)
            {
                behaviour.joinedQueue = true;
                OrderStation.Instance.JoinQueue(behaviour);
            }
        }
    }

    private void TryReserveSlot()
    {
        int slot = OrderStation.Instance.ReserveSlot();
        if (slot == -1)
            return; // still no room in line, try again next frame

        queueSlot = slot;
        slotReserved = true;
        lastSeenQueueVersion = OrderStation.Instance.QueueVersion;
        agent.SetDestination(OrderStation.Instance.lineSlots[queueSlot].transform.position);
    }

    // Called by CustomerBehaviour once a table has been secured and the
    // order is taken. A table is guaranteed non-null here.
    public void LeaveLine(Table table)
    {
        OrderStation.Instance.LeaveQueue(behaviour);
        agent.SetDestination(table.TakeSeat(gameObject).position);
    }
    
    public void Move(Transform target)
    {
        agent.SetDestination(target.position);
    }
}