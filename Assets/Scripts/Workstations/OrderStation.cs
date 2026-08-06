using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderStation : MonoBehaviour
{
    [SerializeField] public Transform[] lineSlots; // Spots for customers to stand in
    [SerializeField] private Image orderImage;

    public List<CustomerBehaviour> customersInLine = new();
    private int reservedSlots;

    // Increments every time a customer leaves the line, so waiting customers
    // know to shift forward exactly once (instead of re-triggering every frame).
    public int QueueVersion { get; private set; }

    public static OrderStation Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (customersInLine.Count > 0 && !customersInLine[0].orderTaken && customersInLine[0].interactable != null)
        {
            customersInLine[0].interactable.enabled = true;
        }
    }

    // Called once per customer at spawn time so two customers spawning in the
    // same frame can never be handed the same physical slot.
    public int ReserveSlot()
    {
        if (reservedSlots >= lineSlots.Length)
            return -1;
        
        // if (customersInLine.Count == 0)
        //     return 0;
        
        int slot = reservedSlots;
        reservedSlots++;
        return slot;
    }

    public void JoinQueue(CustomerBehaviour customer)
    {
        customersInLine.Add(customer);
    }

    public void LeaveQueue(CustomerBehaviour customer)
    {
        customersInLine.Remove(customer);
        PickupStation.Instance.waitingCustomers.Add(customer);
        reservedSlots--;
        QueueVersion++;
    }
}