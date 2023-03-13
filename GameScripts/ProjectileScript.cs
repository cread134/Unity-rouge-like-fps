using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public bool damagesPlayer;
    public bool damagesEnemy;

    [Space]
    public float force;
    public bool continuousForce;
    public float continousForceValue;
    public bool impulseOnSpawn;
    public bool stopRenderOnExplosion;
    [Space]
    public bool stickOnHit = false;
    public bool explodeOnTargetImpact = false;
    [Space]
    public int playerDamage;

    [HideInInspector]
    public float damage;

    public bool destroyOnEnemyCollision;
    public bool damageJustCol;

    public bool noRenderObject;
    public GameObject renderObject;

    public GameObject hitParticle;
    public float hitParticlePercentage;

    public bool noUseDropoff;
    public float dropOffMultiplier;
    public float damageRadius;

    Coroutine lifetimeCoroutine;

    public float maxLifetime;

    public bool destroyOnImpact;
    public GameObject explosionParticle;

    public float lifetimeToExplosion;

    private Rigidbody rb;
    ShootingScript playershoot;

    public LayerMask hitMask;

    public Collider thisCollider;

    public TrailRenderer trail;

    public ParticleSystem accomanyparticles;

    private bool doForce = false;
    // Start is called before the first frame update
    void Awake()
    {
        lifetimeCoroutine = StartCoroutine(Lifetime());
        rb = this.GetComponent<Rigidbody>();

        if(renderObject == null)
        {
            renderObject = this.gameObject;
        }         
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if(continuousForce == true && doForce == true)
        {
            rb.velocity = transform.forward * continousForceValue;
        }
    }

    public void AddVelocity(Vector3 dir, GameObject playerHead)
    {
        rb.velocity = Vector3.zero;
        CancelInvoke();
        StopAllCoroutines();
        thisCollider.enabled = true;
        rb.isKinematic = false;
        if (noRenderObject == false)
        {
            renderObject.GetComponent<MeshRenderer>().enabled = true;
        }
        doForce = true;
        rb.AddForce(dir * force, ForceMode.Impulse);

        StartCoroutine(Lifetime());
        Invoke("ReturnToQueue", lifetimeToExplosion * 2);
        playershoot = playerHead.GetComponent<ShootingScript>();

        if(trail != null)
        {
            trail.Clear();
        }

        if (accomanyparticles != null)
        {
            accomanyparticles.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(stickOnHit == true && collision.transform.CompareTag("Player") == false)
        {
            if (collision.transform.gameObject.layer != 0)
            {
                transform.SetParent(collision.transform);
            }
            rb.isKinematic = true;
            thisCollider.enabled = false;
        }

        if(continuousForce == true)
        {
            continuousForce = false;
        }

        if(damageJustCol == true)
        {
            if (damagesEnemy == true && collision.transform.CompareTag("enemy"))
            {
                collision.transform.gameObject.GetComponent<EnemyLimb>().TakeDamage((int)damage, (this.transform.position - collision.transform.position).normalized, this.transform.position);
            }

            if (collision.transform.CompareTag("explosiveBarrel"))
            {
                collision.transform.gameObject.GetComponent<ExplosiveBarrelScript>().TakeDamage((int)damage);
            }

            if (damagesPlayer == true && collision.transform.CompareTag("Player"))
            {
                collision.transform.gameObject.GetComponent<PlayerHealth>().TakeDamage(playerDamage);
            }

        }

        if(destroyOnImpact == true)
        {
            Explode();
        }
        else
        {
            if (explodeOnTargetImpact == true)
            {
                if (collision.transform.CompareTag("enemy") || collision.transform.CompareTag("explosiveBarrel"))
                {
                    Explode();
                }
                else
                {
                    if (damagesPlayer == true && collision.transform.CompareTag("Player"))
                    {
                        Explode();
                    }
                }
            }
        }
    }

    private void Explode()
    {
        StopAllCoroutines();
        explosionParticle.GetComponent<ParticleSystem>().Play();
    

        if(stopRenderOnExplosion == true && noRenderObject == false)
        {
            renderObject.GetComponent<MeshRenderer>().enabled = false;
        }

        if(accomanyparticles != null)
        {
            accomanyparticles.Stop();
        }
       
        rb.isKinematic = true;
        thisCollider.enabled = false;

        //do the damage
        if (damageJustCol == false)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius, hitMask);
            foreach (Collider col in hitColliders)
            {


                if (damagesPlayer == true && col.transform.CompareTag("Player"))
                {
                    col.transform.gameObject.GetComponent<PlayerHealth>().TakeDamage(playerDamage);
                }


                if (damagesEnemy == true && col.transform.CompareTag("enemy"))
                {
                    if (noUseDropoff != true)
                    {
                        float damageCalc;
                        damageCalc = damage / Vector3.Distance(this.transform.position, col.transform.position) * dropOffMultiplier;
                        Mathf.CeilToInt(damageCalc);
                        int damageToDo = (int)damageCalc;
                        col.transform.gameObject.GetComponent<EnemyLimb>().TakeDamage(damageToDo, (this.transform.position - col.transform.position).normalized, this.transform.position);
                    }
                    else
                    {
                        col.transform.gameObject.GetComponent<EnemyLimb>().TakeDamage((int)damage, (this.transform.position - col.transform.position).normalized, this.transform.position);
                    }


                    //create hit effects

                    if (hitParticle != null)
                    {

                        hitParticle.GetComponent<ParticleSystem>().Play();
                    }
                }


                if (col.transform.CompareTag("explosiveBarrel"))
                {
                    col.transform.gameObject.GetComponent<ExplosiveBarrelScript>().TakeDamage((int)damage);
                }

            }
        }
    }


    public void ReturnToQueue()
    {
        doForce = false;
        thisCollider.enabled = false;
        rb.isKinematic = true;

        if (noRenderObject == false)
        {
            renderObject.GetComponent<MeshRenderer>().enabled = false;
        }
        
        StopAllCoroutines();
        CancelInvoke();
        transform.gameObject.SetActive(false);
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lifetimeToExplosion);
        Explode();
    }
}
