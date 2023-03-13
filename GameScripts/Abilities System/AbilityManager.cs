using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityManager : MonoBehaviour
{
    public MapScript map_script;
    public bool canSwitchAbility = true;

    [HideInInspector]
    public bool usingAbility = false;
    [HideInInspector]
    public bool canUseAbility = true; // for when reloading or melee

    float cooldown;
    float useTime;
    float activateTime;
    int maxCharges;
    bool hasSecondInteraction;
    GameObject abilityObject;
    GameObject abilityPickup;
    Sprite abilityIcon;
    GameObject currentAbilityObject;

    public Image cooldownRepImage;
    public TextMeshProUGUI cooldownText;

    public GameObject abilityHolder; //parent of spawned ability objects


    public  Animator weaponAnimator;


    public AbilityData thisAbility;

    ShootingScript shootScript;
    PickupScript pickupScript;
    PlayerControls p_cont;
    public GameObject controlManager;

    public GameObject body; // the main player body

    private float lastUse;

    public GameObject abilityIconGameobject;

    public Slider AbilitySlider;

    // Start is called before the first frame update
    void Start()
    {
        shootScript = this.GetComponent<ShootingScript>();
        pickupScript = this.GetComponent<PickupScript>();
        p_cont = controlManager.GetComponent<PlayerControls>();

        if(thisAbility != null)
        {
            SwitchActiveAbility(thisAbility);
        }

    }

    public void SwitchActiveAbility(AbilityData abilityToSwitch)
    {
        if (canSwitchAbility) // check to see if we can switch ability
        {
            if(currentAbilityObject != null)
            {
                Instantiate(abilityPickup, this.transform.position, Quaternion.identity);
            }
            cooldown = abilityToSwitch.cooldown;
            useTime = abilityToSwitch.useTime;
            activateTime = abilityToSwitch.activateTime;
            hasSecondInteraction = abilityToSwitch.hasSecondInteraction;
            abilityPickup = abilityToSwitch.abilityPickup;
            abilityObject = abilityToSwitch.abilityObject;
            lastUse = 0;
            abilityIcon = abilityToSwitch.abilityIcon;

            abilityIconGameobject.GetComponent<Image>().sprite = abilityIcon;

            AbilitySlider.maxValue = cooldown;
            AbilitySlider.minValue = 0;

            if (currentAbilityObject != null)
            {
                
                Destroy(currentAbilityObject);
                currentAbilityObject = Instantiate(abilityObject, abilityHolder.transform.position, abilityHolder.transform.rotation, abilityHolder.transform); // creates new ability object
                currentAbilityObject.GetComponent<IAbility>().InitializeAbility(this.gameObject);
              
            }
            else
            {
                currentAbilityObject = Instantiate(abilityObject, abilityHolder.transform.position, abilityHolder.transform.rotation, abilityHolder.transform); // creates new ability object
                currentAbilityObject.GetComponent<IAbility>().InitializeAbility(this.gameObject);
            }
            currentAbilityObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Can't change ability right now");
        }

        canUseAbility = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(Time.time < lastUse)
        {
            AbilitySlider.gameObject.SetActive(true);
            AbilitySlider.value = lastUse - Time.time;

            cooldownText.transform.gameObject.SetActive(true);
            cooldownRepImage.transform.gameObject.SetActive(true);

            float scaledCooldown = (lastUse - Time.time) / cooldown;
            cooldownRepImage.fillAmount = scaledCooldown;
            cooldownText.text = ((int)Mathf.RoundToInt(lastUse - Time.time)).ToString();
        }
        else
        {
            AbilitySlider.gameObject.SetActive(false);

            cooldownText.transform.gameObject.SetActive(false);
            cooldownRepImage.transform.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(p_cont.useAbility) && Time.time > lastUse && canUseAbility && usingAbility == false && this.currentAbilityObject != null && map_script.mapOpen == false) 
        {
            weaponAnimator.enabled = true;
            StartCoroutine(StartAbility()); //starts your ability
        }
        else
        {
            if((Input.GetKeyDown(p_cont.useAbility) && usingAbility == true && hasSecondInteraction == true))
            {
               
                currentAbilityObject.GetComponent<IAbility>().SecondInteration();
            }
        }
    }



    IEnumerator StartAbility() // goes for duration of ability
    {
        
        weaponAnimator.SetBool("WeaponDown", true);
        Debug.Log("started ability");
        usingAbility = true;
        canSwitchAbility = false;
        shootScript.canReload = false;
        shootScript.canShoot = false;

        yield return new WaitForSeconds(0.33f);
        Debug.Log("shoudl Start");

         currentAbilityObject.SetActive(true);
         currentAbilityObject.GetComponent<IAbility>().StartAbility();
          Invoke("UseAbility", activateTime); // starts clock to use

        yield return new WaitForSeconds(useTime + 0.3f);
        weaponAnimator.SetBool("WeaponDown", false);

        yield return new WaitForSeconds(0.33f);
        usingAbility = false;
        canSwitchAbility = true;
        shootScript.canReload = true;
        shootScript.canShoot = true;


        currentAbilityObject.GetComponent<IAbility>().EndAbility();
        currentAbilityObject.SetActive(false);
        lastUse = Time.time + cooldown;
        weaponAnimator.enabled = false;
    }

    private void UseAbility()
    {
        if (usingAbility) // check we are currently in ability state
        {
            currentAbilityObject.GetComponent<IAbility>().UseAbility();
        }
    }
}
