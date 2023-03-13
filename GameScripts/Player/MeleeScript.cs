using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeScript : MonoBehaviour
{

    PlayerControls playerCont;
    ShootingScript shootScript;
    AbilityManager abManager;

    public PlayerMovement p_movement;
    float desiredSpeed;

    public Vector3 meleeHitBox;
    public int meleeDamage;
    public LayerMask meleeMask;
    public float meleeKnockback;

    

    public GameObject[] meleeHitParticles;
    public Animator weaponAnimator;

    public float particleLifetime;
    public GameObject body;

    [Range(0f, 100f)]
    public float particlePercent;
    public int particlePerHit;

    [Range(0f, 2f)]
    public float speedMultiplierWhenMelee = 0.1f;
 

    public GameObject meleeWeapon;
    Animator meleeAnimator;

    public Queue<int> strikes = new Queue<int>(); // if int 0 light strike //if int 1 heavy

    //registering melee

    private float lastMelee = 0;
    public float heavyHoldTime = 0.1f;

    private int lastAnimNum;
    public int maxLightAnims;

    private int combo = 0;

    //bools
    public bool canMelee = true;
    [HideInInspector]
    public bool inMelee = false;
    private bool weaponDown;
    private bool startStrikeAble = false;
    private bool slashing = false;
    public bool doKnocknockback = false;

    public MapScript m_mapscript;


    [Space]
    public ParticleSystem e_hitParticle;

    // Start is called before the first frame update
    void Start()
    {
        GameObject controlsManager = GameObject.FindGameObjectWithTag("ControlsManager");
        playerCont = controlsManager.GetComponent<PlayerControls>();
        meleeAnimator = meleeWeapon.GetComponent<Animator>();
        shootScript = this.GetComponent<ShootingScript>();
        abManager = this.GetComponent<AbilityManager>();

        weaponAnimator.enabled = false;
        lastAnimNum = Random.Range(0, maxLightAnims);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(playerCont.melee) && canMelee == true && shootScript.reloading == false && m_mapscript.mapOpen == false)
        {
    
            lastMelee = Time.time; //register time button press happened
           
        }

        if(Input.GetKeyUp(playerCont.melee) && canMelee == true && strikes.Count < 2 && shootScript.reloading == false && m_mapscript.mapOpen == false)
        {
            float testTime = lastMelee + heavyHoldTime;
            if(Time.time < testTime)
            {
                StrikeLight();
            }
            else
            {
                StrikeHeavy();
            }

            CheckMelee();
        }
    }
    void StrikeLight()
    {
        strikes.Enqueue(0);       
    }

    void StrikeHeavy()
    {
        strikes.Clear();
        strikes.Enqueue(1);
      
    }

    void CheckMelee()
    {
        if(weaponDown == false)
        {
            StopAllCoroutines();
            weaponAnimator.enabled = true;
            StartCoroutine(PutDownWeapon());
        }

        
    }

    IEnumerator PutDownWeapon()
    {
        

        StartMelee();
        combo = 0;
        weaponAnimator.SetBool("WeaponDown", true);

        
        yield return new WaitForSeconds(0.1f);
        startStrikeAble = true;

        StartCoroutine(Slash());
        weaponDown = true;
    }
     
    IEnumerator PutWeaponUp()
    {
        weaponAnimator.SetBool("WeaponDown", false);
        yield return new WaitForSeconds(0.35f);
        StopMelee();
        weaponAnimator.enabled = false;
     //   Debug.Log("done");
    }
    public void StartMelee()
    {
        p_movement.overrideSpeed = true;

        desiredSpeed = p_movement.m_walkSpeed;
        p_movement.m_walkSpeed *= speedMultiplierWhenMelee;

        inMelee = true;

        abManager.canUseAbility = false;

        shootScript.canShoot = false;


        shootScript.canReload = false;
        shootScript.canSwitchWeapon = false;
        weaponDown = true;
    }

    public void StopMelee()
    {
        p_movement.overrideSpeed = false;
        p_movement.m_walkSpeed = desiredSpeed;

        inMelee = false;

        shootScript.canShoot = true;
        shootScript.canReload = true;
        shootScript.canSwitchWeapon = true;
         

        abManager.canUseAbility = true;
        weaponDown = false;
        canMelee = true;
        strikes.Clear();
        combo = 0;
    }

    

   IEnumerator Slash()
    {
        //finding type of strike
        bool isHeavyStrike = false;
        if (strikes.Peek() == 0)
        {
            isHeavyStrike = false;
        }
        else
        {
            isHeavyStrike = true;
        }

        strikes.Dequeue();
        float meleeTime = 0.33f;

        if (isHeavyStrike == false)
        {
            //find slash number
            List<int> choices = new List<int>();
            for (int i = 0; i < maxLightAnims; i++)
            {
                if (i != lastAnimNum)
                {
                    choices.Add(i);
                }
            }

            int animNum = choices[Random.Range(0, choices.Count)];

            lastAnimNum = animNum;
            slashing = true;
            meleeAnimator.SetInteger("LightSlashNum", animNum);
            meleeAnimator.SetTrigger("Slash");
        }
        else
        {
            meleeTime = 1f;
            meleeAnimator.SetTrigger("HeavySlash");
        }
        
        
        yield return new WaitForSeconds(meleeTime / 2);

        RegisterMelee(isHeavyStrike);

        yield return new WaitForSeconds(meleeTime / 2);

     

       
        if(strikes.Count > 0 && isHeavyStrike == false)
        {
   
            StartCoroutine(Slash());
        }
        else
        {
            StartCoroutine(PutWeaponUp());
            slashing = false;
        }                   
    }



    public void RegisterMelee(bool isHeavy)
    {
        int damageTouse = meleeDamage;
        if(isHeavy == true)
        {
            doKnocknockback = true;
            damageTouse *= 2;
        }
        else
        {
            doKnocknockback = false;
        }

        Vector3 pointVector = transform.position + transform.forward * meleeHitBox.z / 2;
        Collider[] hitColliders = Physics.OverlapBox(pointVector, meleeHitBox / 2, transform.rotation, meleeMask);
        if(hitColliders.Length > 0)
        {
            CreateHitEffects();
            combo++;

            e_hitParticle.Play();

            if (hitColliders[0].CompareTag("enemy") && doKnocknockback == true)
            {
                hitColliders[0].gameObject.GetComponent<EnemyLimb>().masterObject.GetComponent<EnemyHealer>().KnockBack(meleeKnockback, transform.forward);
            }                          
           
        }
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("enemy"))
            {
                EnemyLimb e_health = col.gameObject.GetComponent<EnemyLimb>();
                e_health.TakeDamage(damageTouse, transform.forward * -1, col.transform.position);


              //  e_health.StartCoroutine(e_health.KnockBack(meleeKnockback, transform.forward));

                //create particles
                //create hit effects
                for (int i = 0; i < particlePerHit; i++)
                {
                    float particleChance = Random.Range(0f, 100f);
                    if (particleChance <= particlePercent)
                    {

                        GameObject hitToIntantiate = meleeHitParticles[Random.Range(0, meleeHitParticles.Length)];
                        GameObject hitEffect = Instantiate(hitToIntantiate, col.transform.position, Quaternion.LookRotation(transform.forward));
                        Destroy(hitEffect, particleLifetime);
                    }
                }
            }

            if (col.transform.CompareTag("explosiveBarrel"))
            {
                col.transform.gameObject.GetComponent<ExplosiveBarrelScript>().TakeDamage(damageTouse);
                if (isHeavy == true)
                {
                    Rigidbody bartrelRigidbody = col.transform.gameObject.GetComponent<Rigidbody>();
                    bartrelRigidbody.isKinematic = false;
                    bartrelRigidbody.AddForce(transform.forward * meleeKnockback, ForceMode.Impulse);
                }
            }
        }
    }

    void CreateHitEffects()
    {

    }
}
