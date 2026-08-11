using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomerData", menuName = "Scriptable Objects/Customer Data")]
public class CustomerData : ScriptableObject
{
    public float walkSpeed;
    public float servedWaitTime; // How long customer will wait in line till they leave
    public float orderWaitTime; // How long customer will wait for order till they leave
    public float eatTime;
}
