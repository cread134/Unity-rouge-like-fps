using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrelScript : MonoBehaviour
{
    public float radius;
    public GameObject explosionParticle;
    public GameObject renderObject;
    public int damage;
    public int playerDamage = 3;
    ShootingScript playershoot;
    GameObject playerHead;
    public LayerMask hitMask;

    public int health;
    public int startClockHealth;
    public float cooldown;

    bool cooldownStarted = false;

    public GameObject particleObject;

    public AudioClip explodeSound;

    public AudioSource a_source;

    bool exploded = false;

    public void TakeDamage(int damage)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerHead = player.GetComponent<WeaponWhellManager>().headObject;
        playershoot = playerHead.GetComponent<ShootingScript>();

        health -= damage;

        if(health <= 0)
        {
            StopAllCoroutines();
            Explode();
        }
        else
        {
            if(health < startClockHealth)
            {
                if (cooldownStarted == false)
                {
                    StartCoroutine(StartCountdown());
                }
                else
                {
                    Explode();
                }
            }
        }
    }


    public IEnumerator StartCountdown()
    {
        a_source.Play();
        cooldownStarted = true;
        particleObject.SetActive(true);
        yield return new WaitForSeconds(cooldown);
        Explode();
        
    }
    private void Explode()
    {

        if (exploded == false)
        {
            Destroy(particleObject);
            explosionParticle.GetComponent<ParticleSystem>().Play();

            a_source.Stop();
            renderObject.GetComponent<MeshRenderer>().enabled = false;

            a_source.PlayOneShot(explodeSound);

            this.GetComponent<Rigidbody>().isKinematic = true;

            //do the damage
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, hitMask);
            foreach (Collider col in hitColliders)
            {
                if (col.transform.gameObject != this.gameObject)
                {
                    if (col.transform.CompareTag("Player"))
                    {
                        col.transform.gameObject.GetComponent<PlayerHealth>().TakeDamage(playerDamage);
                    }

                    if (col.transform.CompareTag("enemy"))
                    {
            


                        col.transform.gameObject.GetComponent<EnemyLimb>().TakeDamage((int)damage, (this.transform.position - col.transform.position).normalized, this.transform.position);

                    }
                    if (col.transform.CompareTag("explosiveBarrel"))
                    {
                        ExplosiveBarrelScript e_barrelScript = col.transform.gameObject.GetComponent<ExplosiveBarrelScript>();
                        e_barrelScript.StartCoroutine(e_barrelScript.StartCountdown());
                    }


                }
            }
            this.GetComponent<CapsuleCollider>().enabled = false;
            this.GetComponent<Rigidbody>().isKinematic = true;

          
            Destroy(this.gameObject, 2f);
        }
        exploded = true;
    }

}
