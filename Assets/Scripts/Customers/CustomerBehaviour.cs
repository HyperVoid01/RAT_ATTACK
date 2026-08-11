using System.Collections;
using UnityEngine;

public class CustomerBehaviour : MonoBehaviour
{
    [SerializeField] private CustomerData data;
    private Flavour order;
    public bool orderTaken;
    public bool inLine;
    public bool joinedQueue;

    public Flavour Order => order;
    public Interactable interactable;
    public CustomerMovement movement;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        movement = GetComponent<CustomerMovement>();
    }

    private void Start()
    {
        if (interactable != null)
            interactable.enabled = false;

        // Sets a random order
        order = (Flavour)Random.Range(1, 7);
    }

    public void GetServed()
    {
        if (orderTaken)
            return;

        // Secure a table BEFORE committing to anything irreversible.
        // If no table is free, bail out and leave this customer exactly
        // as they were — still in queue, still interactable — so the
        // player (or the next call) can retry once a seat opens up.
        Table table = TableManager.Instance.GetTable();
        if (table == null)
        {
            Debug.Log("No table available yet, customer stays in line");
            return;
        }

        orderTaken = true;
        HUDManager.Instance.AddOrderText(this, order.ToString());

        if (interactable != null)
        {
            interactable.DisableOutline();
            interactable.enabled = false;
        }

        movement.LeaveLine(table);
    }

    public IEnumerator EatPizza(GameObject pizza)
    {
        yield return new WaitForSeconds(data.eatTime);
        Destroy(pizza);
        
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(movement.Leave());
        
    }
}