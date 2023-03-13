using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //setting up script values
    public MapScript m_mapScript;
    public float m_walkSpeed;
    public float groundedHeight;
    public float groundedCheckTime;

    [SerializeField] private float acceleration;

    [SerializeField] private float slopeForce;
    [SerializeField] private float slopeForceRayLength;

    private Rigidbody rb;

    public bool canMove;
    public float gravity;
    public float jumpHeight;

    private Vector3 velocity;

    CharacterController charCont;

    private PlayerControls playerCont;

    public bool grounded;
    private bool doCheckGround = true;

    public int jumpNum;
    private int curJumps = 0;

    private Vector3 moveVector;
    private float airControlSpeed;
    public float airControlMultiplier;

    public Transform gunHolder;
    private Vector3 weaponHolderOrigin;
    private Vector3 weaponHolderTarget;
    public float jumpVisualInertia;
    private float movementCounter;
    private float idleCounter;

    Vector3 targetWeaponBobPos;
    //Jumping
    private bool isJumping;

    public bool canJump = true;

    public GameObject head;
    ShootingScript shootScript;
    public LayerMask groundedLayerMask;

    public bool overrideSpeed = false;

    // Start is called before the first frame update
    void Start()
    {
        airControlSpeed = m_walkSpeed / airControlMultiplier;

        rb = this.GetComponent<Rigidbody>();
        charCont = this.GetComponent<CharacterController>();

        shootScript = head.GetComponent<ShootingScript>();

        //getting the controls
        GameObject controlsManager = GameObject.FindGameObjectWithTag("ControlsManager");
        playerCont = controlsManager.GetComponent<PlayerControls>();

        weaponHolderOrigin = gunHolder.localPosition;
    }

    private void Update()
    {

        if(PauseMenu.gamePaused == true || ShopManager.shopOpen == true)
        {
            canMove = false;
            
        }

        //checking if we are allowed to move
        if (canMove == true && m_mapScript.mapOpen == false)
        {
            Move(m_walkSpeed);
        }

        //checking for grounded
        if (doCheckGround == true && Physics.Raycast(transform.position, Vector3.down, groundedHeight, groundedLayerMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }       

        if(velocity.y > 0)
        {
            isJumping = true;
        }
        if(grounded && velocity.y < 0)
        {
            velocity.y = -2f;
                isJumping = false;
        }

        if (isJumping)
        {
            charCont.slopeLimit = 90f;
        }
        else
        {
            charCont.slopeLimit = 45f;
        }
        if (overrideSpeed != true)
        {
            if (grounded != true)
            {
                m_walkSpeed = airControlSpeed;
            }
            else
            {
                m_walkSpeed = airControlSpeed * airControlMultiplier;
            }
        }

        if (grounded == false)
        {
            weaponHolderTarget = weaponHolderOrigin + new Vector3(0, 0 + jumpVisualInertia * velocity.y);
            weaponHolderTarget = Vector3.ClampMagnitude(weaponHolderTarget, 0.1f);
        }
        else
        {
            weaponHolderTarget = weaponHolderOrigin;
        }

    }

    //moving
    private void Move(float speed)
    {
        //calculating movement vector
        float _xMove = Input.GetAxis("Horizontal");
        float _zMove = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * _xMove;
        Vector3 moveVertical = transform.forward * _zMove;

        charCont.Move((moveVertical + moveHorizontal) * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        charCont.Move(velocity * Time.deltaTime);

        if ((_xMove != 0 || _zMove != 0) && OnSlope())
        {
            charCont.SimpleMove(Vector3.down * charCont.height / 2 * slopeForce);
        }

        JumpInput();


        //headbob
        
        if (_xMove == 0 && _zMove == 0) {

            HeadBob(idleCounter, 0.015f, 0.015f);
            idleCounter += Time.deltaTime;
            gunHolder.localPosition = Vector3.Lerp(gunHolder.localPosition, targetWeaponBobPos, Time.deltaTime * 2f);

        } else {

            HeadBob(movementCounter, 0.025f, 0.025f);
            movementCounter += Time.deltaTime * 5f;
            gunHolder.localPosition = Vector3.Lerp(gunHolder.localPosition, targetWeaponBobPos, Time.deltaTime * 6f);
        }
        
       
    }

    //jumping
    private void JumpInput()
    {
        if (Input.GetKeyDown(playerCont.jump) && isJumping == false && grounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
       
        }
    }


    //fixing slope problems
    private bool OnSlope()
    {
        if ( grounded == false)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, charCont.height / 2f * slopeForceRayLength))
        {
            if (hit.normal != Vector3.up)
            {
                       return true;
            }
        }

        return false;
    }


    void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
    {
        if (shootScript.isShooting == false)
        {
            targetWeaponBobPos = weaponHolderTarget + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0f);
        }
    }
}
