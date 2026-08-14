using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomerData", menuName = "Scriptable Objects/Customer Data")]
public class CustomerData : ScriptableObject
{
    public float walkSpeed;
    public float orderWaitTime; // How long customer will wait for order till they leave
    public float eatTime;
}
