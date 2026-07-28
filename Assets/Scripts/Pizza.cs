using UnityEngine;

public class Pizza : MonoBehaviour
{
    [SerializeField] private Color cookedColor;
    [SerializeField] private Color[] flavorColors = new Color[6];
    
    private PizzaState pizzaState = PizzaState.raw;
    private Flavour flavour = Flavour.Plain;
    
    public PizzaState PizzaState { get => pizzaState; }
    public Flavour Flavour { get => flavour; }

    public void Cook()
    {
        pizzaState = PizzaState.cooked;
        GetComponent<MeshRenderer>().material.color += cookedColor;
    }

    public void ChangeFlavor(Flavour newFlavor)
    {
        flavour = newFlavor;
        GetComponent<MeshRenderer>().material.color = flavorColors[(int)flavour - 1];
    }
}

public enum PizzaState
{
    cooked,
    raw
}

/*
 Different combos:
 0 - Plain
 1 - T1
 2 - T2
 3 - T3
 4 - T1, T2
 5 - T1, T3
 6 - T2, T3
*/

public enum Flavour
{
    Plain,    // No Toppings
    Flavour1, // T1
    Flavour2, // T2
    Flavour3, // T3
    Flavour4, // T1, T2
    Flavour5, // T1, T3
    Flavour6  // T2, T3
}
