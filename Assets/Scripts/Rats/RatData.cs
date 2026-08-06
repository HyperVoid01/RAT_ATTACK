using UnityEngine;

[CreateAssetMenu(fileName = "NewRatData", menuName = "Scriptable Objects/Rat Data")]
public class RatData : ScriptableObject
{
    public float speed; 
    public float wanderRadius;
    public float minIdleTime;
    public float maxIdleTime;
    public float fleeRadius; // How close player must be to trigger fleeing
    public float fleeDistance; // How far the rat tries to run
    public float pizzaRadius; // How close rat detects pizza
    public float eatTime; // How long till pizza is eaten
}
