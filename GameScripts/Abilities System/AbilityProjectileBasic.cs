using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityProjectileBasic : MonoBehaviour, IAbility
{
    Animator thisAnim;

    public GameObject projectile;
    public float bypassrange;
    public Transform spawnLocation;
    public float raycastRange;

    public int damage;

    private GameObject playerhead;

    // Start is called before the first frame update
    void Start()
    {
        thisAnim = this.GetComponent<Animator>();
    }

    void IAbility.InitializeAbility(GameObject playerHead)
    {
        playerhead = playerHead;
    }

    void IAbility.StartAbility()
    {
        if (thisAnim != null)
        {
            thisAnim.SetTrigger(0); // starts the animation
        }
    }

    void IAbility.UseAbility() // calculates the sawning of the projectile
    {

        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, transform.forward, out hit, raycastRange))
        {
            if (Vector3.Distance(hit.point, spawnLocation.transform.position) < bypassrange)
            {
                GameObject projectileInstance = Instantiate(projectile, spawnLocation.transform.position, Quaternion.LookRotation(transform.forward));
                projectileInstance.GetComponent<ProjectileScript>().AddVelocity(transform.forward, playerhead);
                projectileInstance.GetComponent<ProjectileScript>().damage = damage;
            }
            else
            {
                Transform directionToUse = spawnLocation.transform;
                directionToUse.transform.LookAt(hit.point);
                GameObject projectileInstance = Instantiate(projectile, spawnLocation.transform.position, directionToUse.transform.rotation);
                projectileInstance.GetComponent<ProjectileScript>().AddVelocity(directionToUse.forward, playerhead);
                projectileInstance.GetComponent<ProjectileScript>().damage = damage;
            }

        }
        else
        {
            Vector3 pointVector = transform.position + transform.forward * raycastRange;

            Transform directionToUse = spawnLocation.transform;
            directionToUse.transform.LookAt(pointVector);
            GameObject projectileInstance = Instantiate(projectile, spawnLocation.transform.position, directionToUse.transform.rotation);
            projectileInstance.GetComponent<ProjectileScript>().AddVelocity(directionToUse.forward, playerhead);
            projectileInstance.GetComponent<ProjectileScript>().damage = damage;
        }

    }


    void IAbility.EndAbility()
    {

    }

    void IAbility.SecondInteration()
    {

    }
}

