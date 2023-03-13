using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalkerMovement : MonoBehaviour// , //I_Enemy
{
    public EnemyData thisEnemy;

    [HideInInspector]
    public float maxSpeed;
    [HideInInspector]
    public float acceleration;
    [HideInInspector]
    public float forceResistance;
    [HideInInspector]
    public float maxForceThreshold;
    [HideInInspector]
    public bool maintainDistance;
    [HideInInspector]
    public float distanceToPlayer;
    [HideInInspector]
    public float curSpeed;
    private float fleeRange;
    private float minSpeed;
    public float playerMoveRange;
    public float detectionRange;

    private Vector3 playerRangePos;

    NavMeshAgent thisNavmeshAgent;
    Vector3 fleePos;

    public float fleeDamper;

    [HideInInspector]
    public float distance;
    private float distanceInrange;

    [Range(0, 3)]
    public float speedSlowDamper;

    GameObject player;

    public float timeToRefreshPlayerPos;

    public bool canMove = true;

    bool followingPlayer;

    Vector3 startPos;

    Animator thisAnim;

    
    // Start is called before the first frame update
    void Start()
    {
        thisNavmeshAgent = this.transform.gameObject.GetComponent<NavMeshAgent>();
        InitializeValues();
        player = GameObject.FindGameObjectWithTag("Player");

        playerRangePos = player.transform.position;

        startPos = this.transform.position;

        thisAnim = this.GetComponent<Animator>();

    }
    // Update is called once per frame
    void Update()
    {
        if(this.thisNavmeshAgent.velocity != Vector3.zero)
        {
            if (thisAnim != null)
            {
                thisAnim.SetBool("walking", true);
            }
        }
        else
        {
            if (thisAnim != null)
            {
                thisAnim.SetBool("walking", false);
            }
        }
      //  transform.LookAt(player.transform.position);

        if (curSpeed != maxSpeed)
        {
            curSpeed = Mathf.MoveTowards(curSpeed, maxSpeed, maxForceThreshold * Time.deltaTime);
        }
        curSpeed = Mathf.Clamp(curSpeed, minSpeed, maxSpeed);
         distance = Vector3.Distance(player.transform.position, this.transform.position);
        distanceInrange = Vector3.Distance(player.transform.position, playerRangePos);

        thisNavmeshAgent.speed = curSpeed;

        if(distanceInrange > playerMoveRange && canMove == true && followingPlayer == true)
        {
            UpdateDestination();
        }
        if(canMove == true)
        {
            thisNavmeshAgent.isStopped = false;
        }
        else
        {
            thisNavmeshAgent.isStopped = true;
        }

        if (distance > detectionRange)
        {
            followingPlayer = false;
            thisNavmeshAgent.isStopped = true;
            if (thisAnim != null)
            {
                thisAnim.SetBool("deactivated", true);
            }
        }
        
        else
        {
            if (thisAnim != null)
            {
                thisAnim.SetBool("deactivated", false);
            }
            followingPlayer = true;
            if (canMove == false)
            {
                thisNavmeshAgent.isStopped = false;
            }
        }
    }

    void InitializeValues()
    {
        maxSpeed = thisEnemy.maxSpeed;
        curSpeed = maxSpeed;
        acceleration = thisEnemy.acceleration;
        forceResistance = thisEnemy.forceResistance;
        maxForceThreshold = thisEnemy.maxForceThreshold;
        maintainDistance = thisEnemy.maintainDistance;
        distanceToPlayer = thisEnemy.distanceToPlayer;
        fleeRange = thisEnemy.fleeRange;
        thisNavmeshAgent.acceleration = acceleration;
        minSpeed = thisEnemy.minSpeed;
    }



    public void LoseSpeed(float amount)
    {
        curSpeed -= amount * speedSlowDamper;
        
    }
    
    public void UpdateDestination()
    {

        switch (maintainDistance)
        {
            case true:
                if (distance < distanceToPlayer)
                {

                    Vector3 dirToPlayer = (transform.position - player.transform.position).normalized * fleeDamper;
                    Vector3 newPos = transform.position + dirToPlayer;

                    thisNavmeshAgent.destination = newPos;

                }
                else
                {
                    if (distance > distanceToPlayer + 5)
                    {
                        Vector3 dirToPlayer = (transform.position - player.transform.position).normalized * distanceToPlayer;
                        Vector3 newPos = player.transform.position + dirToPlayer;
                        thisNavmeshAgent.destination = newPos;
                    }

                }
                break;
            case false:

                thisNavmeshAgent.destination = player.transform.position;
                break;
        }
       
        playerRangePos = player.transform.position;
    }

   // public void OnSpawn()
  //  {
   //     StartCoroutine(Spawned());
   // }
//
    IEnumerator Spawned()
    {
        canMove = false;
        yield return new WaitForSeconds(1f);
        canMove = true;
        UpdateDestination();
    }
}
