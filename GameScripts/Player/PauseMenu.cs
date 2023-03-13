using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public static bool gamePaused = false;

    public GameObject pauseMenu;
    public GameObject playerhead;
    public GameObject player;
    public GameObject leaveToMenu;
    GameObject levelLoader;

    MouseLook mouseLook;
    ShootingScript shootScript;
    PlayerMovement p_movement;
    DashScript d_script;

    // Start is called before the first frame update
    void Start()
    {

        mouseLook = playerhead.GetComponent<MouseLook>();
        shootScript = playerhead.GetComponent<ShootingScript>();
        p_movement = player.GetComponent<PlayerMovement>();
        d_script = player.GetComponent<DashScript>();

        levelLoader = GameObject.FindGameObjectWithTag("levelLoader");
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if(gamePaused == true)
            {
                Resume();        
            }
            else        
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
        leaveToMenu.SetActive(false);

        mouseLook.lockCursor = true;
        mouseLook.canLook = true;

        shootScript.canShoot = true;
        Cursor.visible = false;

        p_movement.canMove = true;
        d_script.canDash = true;
    }

    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
        player.GetComponent<WeaponWhellManager>().inWheel = false;


        Cursor.visible = true;
    }

    public void LeaveGame()
    {
        leaveToMenu.SetActive(true);
    }
    public void ReturnToPause()
    {
        leaveToMenu.SetActive(false);
    }
    public void GoToMenu()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        levelLoader.GetComponent<LevelLoader1>().LoadMenu();
    }
}
