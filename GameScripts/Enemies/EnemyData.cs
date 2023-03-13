using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{
    public int health;
    public int attackDamage;

    public GameObject corpse;

    //movement
    public float maxSpeed;
    public float acceleration;
    public float forceResistance;
    public float maxForceThreshold;
    public bool maintainDistance;
    public float distanceToPlayer;
    public float fleeRange;
    public float minSpeed;
}
