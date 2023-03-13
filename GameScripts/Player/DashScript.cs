using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashScript : MonoBehaviour
{

    public GameObject representor1;
    public GameObject representor2;

    public bool canDash = true;

    [Tooltip("smaller is longer cooldown")]
    public float coolDown;
    public float coolDownRepresenatation;
    public float dashTime;
    public float dashDistance;

    [HideInInspector]
    public int currentDashes;
    private float currentCooldownValue;

    private bool doCooldown = true;
    private PlayerControls playerCont;
    private CharacterController thisChar;

    private Vector3 dashVector;

    float doTime;
    private bool isDashing;

    private Vector3 dashHorizontal;
    private Vector3 dashVertical;

    [Header("Effects")]
    public GameObject forwardEffect;
    public GameObject leftEffect;
    public GameObject rightEffect;
    private ParticleSystem forwardParticle;
    private ParticleSystem leftParticle;
    private ParticleSystem rightParticle;

    // Start is called before the first frame update
    void Start()
    {
        coolDownRepresenatation = coolDown * 20f;

   

        //getting the controls
        GameObject controlsManager = GameObject.FindGameObjectWithTag("ControlsManager");
        playerCont = controlsManager.GetComponent<PlayerControls>();
        thisChar = this.GetComponent<CharacterController>();

        //setting up dash Effects
        forwardParticle = forwardEffect.GetComponent<ParticleSystem>();
        leftParticle = leftEffect.GetComponent<ParticleSystem>();
        rightParticle = rightEffect.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseMenu.gamePaused == true || ShopManager.shopOpen == true)
        {
            canDash = false;
        }


        //calculating movement vector
        float _xMove = Input.GetAxisRaw("Horizontal");
        float _zMove = Input.GetAxisRaw("Vertical");

        if(_zMove == 0 && _xMove == 0)
        {
            dashVertical = transform.forward;
           dashHorizontal = Vector3.zero;
        }
        else
       {
             dashHorizontal = transform.right * _xMove;
             dashVertical = transform.forward * _zMove;
       }
 

        

        if(isDashing == true)
        {
            thisChar.Move(dashVector);
        }

        if (Input.GetKeyDown(playerCont.dash) && canDash == true && currentDashes > 0){
            StartCoroutine(IsDashing());
            if(_zMove != 0)
            {
                forwardParticle.Play();
            }
            else
            {
                if(_xMove > 0)
                {
                    rightParticle.Play();
                }
                else
                {
                    leftParticle.Play();
                }
            }
        }

        if (currentDashes < 2 && doCooldown == true)
        {
            currentCooldownValue = Mathf.MoveTowards(currentCooldownValue, 2f, Time.deltaTime * coolDown);
        }


        
        if (currentCooldownValue - 1f >= 1)
        {
            currentDashes = 2;
            representor2.SetActive(true);
        }
        else
        {
            representor2.SetActive(false);
            if (currentCooldownValue >= 1)
            {
                currentDashes = 1;
                representor1.SetActive(true);
            }
            else
            {
                
                representor1.SetActive(false);
            }
        }
    }


    IEnumerator IsDashing()
    {
        dashVector = (dashHorizontal + dashVertical) * dashDistance;
        currentDashes--;
        currentCooldownValue -= 1f;
        doCooldown = false;
        canDash = false;
        isDashing = true;

        yield return new WaitForSeconds(dashTime);
        doCooldown = true;
        canDash = true;
        isDashing = false;
    }
}
