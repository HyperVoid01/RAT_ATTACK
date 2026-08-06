using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class IngredientStation : MonoBehaviour
{
    [SerializeField] private Interactable topping1;
    [SerializeField] private Interactable topping2;
    [SerializeField] private Interactable topping3;

    private GameObject currentPizzaObject;
    private Pizza currentPizza;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pizza") && !currentPizza && other.gameObject != currentPizzaObject)
        {
            // You can only add toppings to uncooked pizza
            if (other.GetComponent<Pizza>().PizzaState != PizzaState.raw)
                return;
            
            currentPizzaObject = other.gameObject;
            currentPizza = currentPizzaObject.GetComponent<Pizza>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pizza") && other.gameObject == currentPizzaObject)
        {
            currentPizza = null;
            currentPizzaObject = null;
        }
    }

    public void AddTopping1()
    {
        if (!currentPizza)
            return;

        switch (currentPizza.Flavour)
        {
            case Flavour.Plain:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour1);
                break;
            }

            case Flavour.Flavour2:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour4);
                break;
            }

            case Flavour.Flavour3:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour5);
                break;
            }
            
        }
    }

    public void AddTopping2()
    {
        if (!currentPizza)
            return;
        
        switch (currentPizza.Flavour)
        {
            case Flavour.Plain:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour2);
                break;
            }
            
            case Flavour.Flavour1:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour4);
                break;
            }
            
            case Flavour.Flavour3:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour6);
                break;
            }
        }
    }

    public void AddTopping3()
    {
        if (!currentPizza)
            return;
        
        switch (currentPizza.Flavour)
        {
            case Flavour.Plain:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour3);
                break;
            }
            
            case Flavour.Flavour1:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour5);
                break;
            }
            
            case Flavour.Flavour2:
            {
                currentPizza.ChangeFlavor(Flavour.Flavour6);
                break;
            }
        }
    }
}
