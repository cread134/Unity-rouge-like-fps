using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShootingScript : MonoBehaviour
{
    // SoundManager soundmanager;

    public MapScript m_script;

    public string lineRenderName;

    public LayerMask bypass;

    public bool canShoot;


    ParticleSystem muzzleFlash;

    GameObject barrelend;

    public Animator gunAnimator;

    [HideInInspector]
    public GameObject currentWeaponModel;

    public GameObject gunPos;
    public GameObject gunPosHolder;
    public GameObject headHolder;
    AdcancedCameraRecoil advancedCam;
    AdvancedRecoil adv_recoil;

    public GunScript curGun;

    [HideInInspector]
    public bool reloading = false;

    private float lastShot = 0f;



    PlayerControls playerCont;
    AmmunitionManager ammoManager;
    WeaponWhellManager weaponWheelManager;
    PlayerNoise p_noise;

    //Ui components
    public GameObject ammoReserveText;
    public GameObject ammunitionText;
    public GameObject reloadText;
    public GameObject body;
    public Slider ammoSlider;

    public Image weaponIconRepresentor;

    public CrossHairScript crosshairScript;

    ObjectPooler objectPooler;

    private Coroutine reloadCoroutine;

    public float particleLifetime;

    public bool canReload = true;
    public bool canSwitchWeapon = true;


    //ammo ui
    public GameObject ammoUIHolder;
    Image ammoUiSprite;
    public Sprite smallAmmo;
    public Sprite largeAmmo;
    public Sprite shotgunAmmo;
    public Sprite specialAmmo;


    [HideInInspector]
    public AbilityManager abManager;

    [HideInInspector]
    public bool isShooting;
    private float lastTimeShooting;
    private bool heldDown;
    [HideInInspector]
    public bool sequentialinprocess = false;

    //drawwing weapon
    private Coroutine drawCoroutine;

    private bool shootOverride;

    public MeleeScript meleeScript;

    float lastHitSound;
    public AudioClip hitSound;

    bool updateAmmoAnimator = false;
    GunModelScript g_ModelScript;

    public Queue<GameObject> projectileQueue = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        ammoUiSprite = ammoUIHolder.GetComponent<Image>();
        GameObject controlsManager = GameObject.FindGameObjectWithTag("ControlsManager");
        playerCont = controlsManager.GetComponent<PlayerControls>();
        ammoManager = this.GetComponent<AmmunitionManager>();
        weaponWheelManager = body.GetComponent<WeaponWhellManager>();
        LoadNewGun(weaponWheelManager.wheelSlots[0].GetComponent<WheelSlotScript>().slotGun);
        abManager = this.GetComponent<AbilityManager>();
        adv_recoil = gunPosHolder.GetComponent<AdvancedRecoil>();
        advancedCam = headHolder.GetComponent<AdcancedCameraRecoil>();
        p_noise = GetComponent<PlayerNoise>();

        objectPooler = ObjectPooler.Instance;

       // soundmanager = FindObjectOfType<SoundManager>();
    }

    public void LoadNewGun(GunScript gun)
    {
        if (canSwitchWeapon == true)
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
            }
            

            if (currentWeaponModel != null)
            {
               // curGun.curAmmo = curmag;
                Destroy(currentWeaponModel);
            }
            currentWeaponModel = Instantiate(gun.weaponModel, gunPos.transform.position, gunPos.transform.rotation, gunPos.transform);
            g_ModelScript = currentWeaponModel.GetComponent<GunModelScript>();

            curGun = gun;
 

            //here is all the values

            barrelend = currentWeaponModel.GetComponent<GunModelScript>().barrelEnd;

            ammoSlider.maxValue = curGun.maxAmmo;


            crosshairScript.UpdateCrosshair(curGun.crosshairSizeMultiplier, curGun.crossHairReturnSpeed, curGun.crosshairSizeClamp,curGun.crosshair);


            //handle audio

            weaponIconRepresentor.sprite = curGun.w_Icon;
            

            if (curGun.hasMuzzleFlash == true)
            {
                muzzleFlash = barrelend.transform.GetChild(0).GetComponent<ParticleSystem>();
            }
            else
            {
                muzzleFlash = null;
            }

            gunAnimator  = currentWeaponModel.GetComponent<Animator>();


            int ammoReserves = ammoManager.quantity[curGun.ammoIndex];
            //handling ui
            ammunitionText.GetComponent<TextMeshProUGUI>().text = curGun.curAmmo.ToString();
            ammoReserveText.GetComponent<TextMeshProUGUI>().text = ammoReserves.ToString();

            //set the ammoicons
            switch (curGun.ammoIndex)
            {
                case 0:
                    ammoUiSprite.sprite = smallAmmo;
                    break;

                case 1:
                    ammoUiSprite.sprite = largeAmmo;
                    break;

                case 2:
                    ammoUiSprite.sprite = specialAmmo;

                    break;
                case 3:
                    ammoUiSprite.sprite = shotgunAmmo;
                    break;

            }

            //handle drawing of gun
            if (drawCoroutine != null)
            {
                StopCoroutine(drawCoroutine);
                StartCoroutine(DrawWeapon());
            }
            else
            {
                StartCoroutine(DrawWeapon());
            }

            updateAmmoAnimator = g_ModelScript.updateAmmoAnimator;

            if(curGun.raycast == false)
            {
                CreateProjectileQueue();
            }
        }
    }

    //projectile pooling
    void CreateProjectileQueue()
    {
        if (projectileQueue.Count != 0)
        {
            foreach (GameObject g in projectileQueue)
            {
                Destroy(g);
            }
        }
        
        projectileQueue.Clear();

        for (int i = 0; i < curGun.amountToQueue; i++)
        {
            GameObject projInstance = Instantiate(curGun.projectile, transform);
            projectileQueue.Enqueue(projInstance);
            projInstance.GetComponent<ProjectileScript>().damage = curGun.damage;
            projInstance.GetComponent<ProjectileScript>().ReturnToQueue();
            
        }       
    }

    void CreateProjectile(Vector3 position, Quaternion rotation, Vector3 velocityDir)
    {
        
        GameObject toUse = projectileQueue.Peek();
        toUse.transform.SetParent(null);
        toUse.SetActive(true);
        toUse.transform.position = position;
        toUse.transform.rotation = rotation;

        toUse.GetComponent<ProjectileScript>().AddVelocity(velocityDir, gameObject);

        projectileQueue.Dequeue();
        projectileQueue.Enqueue(toUse);
    }

    IEnumerator DrawWeapon()
    {
        shootOverride = true;
        canShoot = false;
        canReload = false;
        yield return new WaitForSeconds(curGun.drawTime);

        shootOverride = false;
        canShoot = true;
        canReload = true;
        meleeScript.canMelee = true;
    }

    // Update is called once per frame
    void Update()
    {
        

        if(PauseMenu.gamePaused == true)
        {
            canShoot = false;
        }

        if(Time.time > lastTimeShooting)
        {
            isShooting = false;
        }
        else
        {
            isShooting = true;
        }




        if (shootOverride)
        {
            canShoot = false;
        }

        if(reloading != true && curGun.curAmmo == 0)
        {
            reloadText.SetActive(true);
        }
        else
        {
            reloadText.SetActive(false);
        }

 

        //stopping sequential
        if (Input.GetKeyDown(playerCont.attack) && sequentialinprocess == true)
        {
            canShoot = true;
            reloading = false;
            gunAnimator.SetBool("Full", true);

            abManager.canUseAbility = true;
            sequentialinprocess = false;
        }

        if (Input.GetKey(playerCont.attack) && Time.time > lastShot && canShoot == true && curGun.curAmmo > 0 && curGun.automatic == true && reloading == false && m_script.mapOpen == false)
        {
            //animations 
            //handling animation
            if (gunAnimator != null)
            {
                gunAnimator.SetTrigger("Shoot");
            }

            lastShot = Time.time + curGun.firerate;

            //muzzle flash
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            if (curGun.shootSound != null)
            {
                p_noise.PlaySound(curGun.shootSound, curGun.pitchShift);
            }
            for (int i = 0; i < curGun.bulletsPerShot; i++)
            {

                Shootdata();
            }

            //handling ammunition

            if (curGun.ammoPerShot > 1)
            {
                int ammoRevision = curGun.curAmmo - curGun.ammoPerShot;
                if (ammoRevision > 0)
                {
                    curGun.curAmmo -= curGun.ammoPerShot;
                }
                else
                {
                    curGun.curAmmo += ammoRevision;
                }
            }
            curGun.curAmmo -= curGun.ammoPerShot;

        }

        if (Time.time > lastShot && canShoot == true && Input.GetKeyDown(playerCont.attack) && curGun.curAmmo > 0 && curGun.automatic == false && reloading == false && m_script.mapOpen == false)
        {
            //handling animation
            if(gunAnimator != null)
            {
                gunAnimator.SetTrigger("Shoot");
            }
            else
            {
                Debug.Log("null animator");
            }


            //handling ammunition
            if (curGun.ammoPerShot > 1)
            {
                int ammoRevision = curGun.curAmmo - curGun.ammoPerShot;
                if (ammoRevision > 0)
                {
                   curGun.curAmmo -= curGun.ammoPerShot;
                }
                else
                {
                    curGun.curAmmo += ammoRevision;
                }
            }
            curGun.curAmmo -= curGun.ammoPerShot;
            lastShot = Time.time + curGun.firerate;

            //muzzle flash
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            if (curGun.shootSound != null)
            {
                p_noise.PlaySound(curGun.shootSound, curGun.pitchShift);
            }

            for (int i = 0; i < curGun.bulletsPerShot; i++)
            {
                Shootdata();
            }
           
        }

        

        //handling ui
        ammunitionText.GetComponent<TextMeshProUGUI>().text = curGun.curAmmo.ToString();

        int ammoReserves = ammoManager.quantity[curGun.ammoIndex];
        ammoReserveText.GetComponent<TextMeshProUGUI>().text = ammoReserves.ToString();

        int ammoSubtract = curGun.maxAmmo - curGun.curAmmo;
        ammoSlider.value = ammoSubtract;

        //startreload
        if (Input.GetKeyDown(playerCont.reload) && curGun.curAmmo < curGun.maxAmmo && reloading != true && ammoManager.quantity[curGun.ammoIndex] > 0 && canReload == true && m_script.mapOpen == false)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }
    }

    void Shootdata()
    {
        lastTimeShooting = Time.time + 0.7f;

        //handling accuracy
        Vector3 forwardVector = Vector3.forward;
        float deviation = Random.Range(0f, curGun.accuracy);
        float angle = Random.Range(0f, 360f);
        forwardVector = Quaternion.AngleAxis(deviation, Vector3.up) * forwardVector;
        forwardVector = Quaternion.AngleAxis(angle, Vector3.forward) * forwardVector;
        forwardVector = this.transform.rotation * forwardVector;
        if (curGun.raycast && curGun.customShooting == false)
        {
            ShootRaycast(forwardVector);
        }
        else
        {
            if (curGun.customShooting == false)
            {
                ShootProjectile(forwardVector);
            }
            else
            {
                currentWeaponModel.GetComponent<GunInterface>().Shoot();
            }
        }

 
        //creates procedural weapon recoil
        adv_recoil.Fire(curGun.recoilRotation);
        advancedCam.Fire(curGun.recoilRotationCam, curGun.recoilKickBack, curGun.returnSpeed, curGun.rotationSpeed);

        if (curGun.sequentialReload)
        {
            gunAnimator.SetBool("Full", true);
        }


        if(updateAmmoAnimator == true)
        {
            g_ModelScript.UpdateAmmoNotifier(curGun.curAmmo);
        }

        crosshairScript.AddCrosshair(curGun.crossHairRecoil);
    }

    void ShootRaycast(Vector3 forwardVector)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, forwardVector, out hit, curGun.raycastRange, bypass))
        {


            GameObject lineR = objectPooler.SpawnFromPool(lineRenderName, barrelend.transform.position, barrelend.transform.rotation, null);

            lineR.GetComponent<LineRScript>().MoveToPoint(hit.point);
            //lineR.GetComponent<LineRScript>().MoveToPoint(spawnVector);
            LineRenderer lineInstanceRenderer = lineR.GetComponent<LineRenderer>();
            lineInstanceRenderer.colorGradient = curGun.trailColor;
            lineInstanceRenderer.widthMultiplier = curGun.trailWidth;

            if (hit.transform.CompareTag("enemy"))
            {

                if(Time.time > lastHitSound)
                {
                    p_noise.PlaySound(hitSound, 0.01f);
                    lastHitSound = Time.time + 0.05f;
                }


                if (curGun.damageFalloff == true)
                {
                    float d_amount = curGun.damage - Vector3.Distance(this.transform.position, hit.point) * curGun.falloffMultiplier;
                    int doDamage = (int)Mathf.CeilToInt(d_amount);

                    if (doDamage <= 0)
                    {
                        doDamage = 1;
                    }
                    hit.transform.gameObject.GetComponent<EnemyLimb>().TakeDamage(doDamage, forwardVector * -1, hit.point);
                }
                else
                {
                    hit.transform.gameObject.GetComponent<EnemyLimb>().TakeDamage(curGun.damage, forwardVector * -1,  hit.point);
                }

                //create hit effects
                float particleChance = Random.Range(0f, 100f);
                if (particleChance <= curGun.hitParticlePercentage)
                {
                    if (curGun.enemyHitParticles.Length > 0)
                    {
                        GameObject hitToIntantiate = curGun.enemyHitParticles[Random.Range(0, curGun.enemyHitParticles.Length)];
                        GameObject hitEffect = Instantiate(hitToIntantiate, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(hitEffect, particleLifetime);
                    }
                }
            }
            else
            {
                if (hit.transform.CompareTag("explosiveBarrel"))
                {
                    hit.transform.gameObject.GetComponent<ExplosiveBarrelScript>().TakeDamage(curGun.damage);
          
          

                    if (curGun.impactEffect != null)
                    {
                        GameObject hitEffect = Instantiate(curGun.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(hitEffect, particleLifetime);
                    }
                }
                else
                {
                    if (curGun.impactEffect != null)
                    {
                        GameObject hitEffect = Instantiate(curGun.impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(hitEffect, particleLifetime);
                    }
                }
            }
        }
        else
        {
            Vector3 spawnVector = this.transform.position + forwardVector * curGun.raycastRange;
            GameObject lineR = objectPooler.SpawnFromPool(lineRenderName, barrelend.transform.position, barrelend.transform.rotation, null);
            lineR.GetComponent<LineRScript>().MoveToPoint(spawnVector);
            LineRenderer lineInstanceRenderer = lineR.GetComponent<LineRenderer>();
           lineInstanceRenderer.colorGradient = curGun.trailColor;
            lineInstanceRenderer.widthMultiplier = curGun.trailWidth;
        }
    }

    void ShootProjectile(Vector3 forwardVector)
    {


        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, forwardVector,out hit, curGun.raycastRange))
        {
            if(Vector3.Distance(hit.point, barrelend.transform.position) < curGun.bypassRange)
            {
                CreateProjectile(barrelend.transform.position, Quaternion.LookRotation(forwardVector), forwardVector); 
            }
            else
            {
                Transform directionToUse = barrelend.transform;
                directionToUse.transform.LookAt(hit.point);
                CreateProjectile(barrelend.transform.position, directionToUse.transform.rotation, directionToUse.transform.forward);
            }

        }
        else
        {
            Vector3 pointVector = transform.position + forwardVector * curGun.raycastRange;

            Transform directionToUse = barrelend.transform;
            directionToUse.transform.LookAt(pointVector);
            CreateProjectile(barrelend.transform.position, directionToUse.transform.rotation, directionToUse.transform.forward);
        }       
      
    }

    IEnumerator Reload()
    {
        if (curGun.sequentialReload)
        {
            gunAnimator.SetBool("Full", false);
        }
        reloading = true;
        canShoot = false;

        abManager.canUseAbility = false;

        if (gunAnimator != null)
        {
            gunAnimator.SetTrigger("Reload");
        }
        if (curGun.reloadClip != null)//play reload sound
        {
            p_noise.PlaySound(curGun.reloadClip, 0);
        }
        yield return new WaitForSeconds(curGun.reloadTime);

        if (curGun.sequentialReload == false)
        {

            int ammoReserveCount = curGun.maxAmmo - curGun.curAmmo;
            if (ammoManager.quantity[curGun.ammoIndex] >= ammoReserveCount)
            {
                ammoManager.quantity[curGun.ammoIndex] -= ammoReserveCount;
                curGun.curAmmo = curGun.maxAmmo;

            }
            else
            {
                curGun.curAmmo = ammoManager.quantity[curGun.ammoIndex];
                ammoManager.quantity[curGun.ammoIndex] = 0;
            }

            shootOverride = false;
            canShoot = true;
            reloading = false;

            int ammoReserves = ammoManager.quantity[curGun.ammoIndex];
            ammoReserveText.GetComponent<TextMeshProUGUI>().text = ammoReserves.ToString();


            abManager.canUseAbility = true;
        }
        else
        {
            StartCoroutine(SequentialReload());
        }

        if (updateAmmoAnimator == true)
        {
            g_ModelScript.UpdateAmmoNotifier(curGun.curAmmo);
        }
    }

    IEnumerator SequentialReload()
    {
    
        if (curGun.curAmmo < curGun.maxAmmo)
        {
            sequentialinprocess = true;
        }

       // gunAnimator.SetTrigger("sequential");

        yield return new WaitForSeconds(curGun.sequentialTime);

        if (sequentialinprocess == true)
        {
            if (ammoManager.quantity[curGun.ammoIndex] > 0)
            {
                curGun.curAmmo++;
                ammoManager.quantity[curGun.ammoIndex]--;
            }
            else
            {

                ammoManager.quantity[curGun.ammoIndex] = 0;
                shootOverride = false;
                canShoot = true;
                reloading = false;
                gunAnimator.SetBool("Full", true);
                
                int ammoReserves = ammoManager.quantity[curGun.ammoIndex];
                ammoReserveText.GetComponent<TextMeshProUGUI>().text = ammoReserves.ToString();
  

                abManager.canUseAbility = true;
                sequentialinprocess = false;

   
                yield return null;
            }
            
            if (curGun.curAmmo == curGun.maxAmmo)
            {
                //this is big gay but hey it works
                gunAnimator.SetBool("Full", true);

                curGun.curAmmo = curGun.maxAmmo;
                shootOverride = false;
                canShoot = true;
                reloading = false;

                int ammoReserves = ammoManager.quantity[curGun.ammoIndex];
                ammoReserveText.GetComponent<TextMeshProUGUI>().text = ammoReserves.ToString();

                abManager.canUseAbility = true;
                sequentialinprocess = false;          

            }
            else
            {
                gunAnimator.SetBool("Full", false);
                StartCoroutine(SequentialReload());
                
            }
        }
        else
        {
            if(ammoManager.quantity[curGun.ammoIndex] == 0)
            {
                
                ammoManager.quantity[curGun.ammoIndex] = 0;
                shootOverride = false;
                canShoot = true;
                reloading = false;
                gunAnimator.SetBool("Full", true);
                
                int ammoReserves = ammoManager.quantity[curGun.ammoIndex];
                ammoReserveText.GetComponent<TextMeshProUGUI>().text = ammoReserves.ToString();

                abManager.canUseAbility = true;
                sequentialinprocess = false;

            }
        }
    }

    public void UpdateGunInfo()
    {
        ammunitionText.GetComponent<TextMeshProUGUI>().text = curGun.curAmmo.ToString();

        int ammoSubtract = curGun.maxAmmo - curGun.curAmmo;
        ammoSlider.value = ammoSubtract;
    }

}
