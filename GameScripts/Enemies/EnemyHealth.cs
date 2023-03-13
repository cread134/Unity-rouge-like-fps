using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{

    public EnemyData thisEnemy;
    GameObject player;

    private int health;
    private int lastDamage;

    ObjectPooler objectPooler;

    public GameObject damageTextPos;

    private GameObject corpse;
    private float forceResistance;
    public bool canDie = true;
    public float knockBackTime;
    public float thisEnemyHeight;

    float lastDamageTime;

    private Vector3 damage_direction;

    [HideInInspector]
    public GameObject roomIn; // the room this enemy was spawned in

    EnemyWalkerMovement e_walkerMovement;
    public LayerMask corpseMask;

    public GameObject[] gores;

    private bool returnNavmesh;

    float lastHit;
    Animator thisAnimator;

    //loot drops
    public Transform dropPosition;

    [Range(0, 1)]
    public float ammoSpawn;
    [Range(0, 1)]
    public float healthSpawn;

    public GameObject[] ammoDrops;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        health = thisEnemy.health;
        corpse = thisEnemy.corpse;
        forceResistance = thisEnemy.forceResistance;
        objectPooler = ObjectPooler.Instance;
        lastDamageTime = Time.time;

        e_walkerMovement = this.GetComponent<EnemyWalkerMovement>();
        thisAnimator = this.GetComponent<Animator>();
    }

    public void TakeDamage(int damage, Vector3 damageDirection, bool isExplosion, Vector3 hitPoint)
    {
        lastHit = Time.time + 0.1f;

        health -= damage;

        //create damage numbers
        GameObject d_text = objectPooler.SpawnFromPool("damageText", damageTextPos.transform.position, Quaternion.identity, null);
        GameObject dTexttext = d_text.transform.GetChild(0).gameObject;
        dTexttext.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        damage_direction = damageDirection;
        lastDamage = lastDamage + damage;

        if (health <= 0 && canDie == true)
        {
            canDie = false;
            Die(isExplosion, hitPoint);
        }

        e_walkerMovement.LoseSpeed(damage / forceResistance);
    }

    public void Die(bool isExplosion, Vector3 point)
    {

        CreateDrops(lastDamage);


        if (lastDamage != 0 && isExplosion == false)
        {
            GameObject corpseInstance = Instantiate(corpse, this.transform.position, this.transform.rotation);

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f, corpseMask);
            foreach(Collider col in hitColliders)
            {
                if (col.gameObject.GetComponent<Rigidbody>() != null)
                {
                    col.GetComponent<Rigidbody>().AddForce(damage_direction * -1 * (lastDamage * forceResistance), ForceMode.Impulse);
              
                }
            }

            if(hitColliders == null)
            {
                corpseInstance.GetComponent<Rigidbody>().AddForce(damage_direction * -1 * (lastDamage * forceResistance), ForceMode.Impulse);
            }
        }
        else
        {
            int goreAmount = Random.Range(3, 6);
            for (int i = 0; i < goreAmount; i++)
            {
                GameObject toInstantiate = gores[Random.Range(0, gores.Length)];
                GameObject goreInstance = Instantiate(toInstantiate, this.transform.position, this.transform.rotation);
                goreInstance.GetComponent<Rigidbody>().AddForce(damage_direction * -1 * (lastDamage / forceResistance), ForceMode.Impulse);
            }
        }

        if(roomIn != null)
        {
            roomIn.GetComponent<RoomClass>().spawnedEnemies.Remove(this.gameObject);

        }


        Destroy(this.gameObject);

       
    }

    public IEnumerator KnockBack(float amount, Vector3 direction)
    {
        this.transform.LookAt(player.transform.position);
        thisAnimator.SetBool("knockedback", true);
        e_walkerMovement.canMove = false;
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<NavMeshAgent>().enabled = false;    

        this.GetComponent<Rigidbody>().AddForce(direction * amount, ForceMode.Impulse);

        yield return new WaitForSeconds(knockBackTime);
        thisAnimator.SetBool("knockedback", false);
        returnNavmesh = true;

       
        
    }

    private void Update()
    {
         if(returnNavmesh == true)
        {
            if(Physics.Raycast(transform.position, -Vector3.up, thisEnemyHeight + 0.1f))
            {
                this.GetComponent<NavMeshAgent>().enabled = true;
                this.GetComponent<Rigidbody>().isKinematic = true;

                returnNavmesh = false;
                e_walkerMovement.canMove = true;
               
            }
        }
         if(lastHit< Time.time)
        {
            lastDamage = 0;
        }

        lastDamage = Mathf.Clamp(lastDamage, 0, 100);
    }

    public void CreateDrops(float lastD)
    {
        int ammoNum = (int)Mathf.RoundToInt(ammoSpawn * 10);
        int healthNum = (int)healthSpawn * 10;


        for (int i = 0; i < ammoNum; i++)
        {
            GameObject ammoToDrop = ammoDrops[Random.Range(0, ammoDrops.Length)];
            Quaternion spawnRot = Random.rotation;

            Transform lookTransform = dropPosition;
            lookTransform.rotation = spawnRot;

            GameObject justSpawned = Instantiate(ammoToDrop, dropPosition.position, Quaternion.identity);
            justSpawned.GetComponent<Rigidbody>().AddForce(lookTransform.forward * Mathf.Sqrt(lastD), ForceMode.Impulse);
        }
    }
}
