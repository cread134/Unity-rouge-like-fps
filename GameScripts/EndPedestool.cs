using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPedestool : MonoBehaviour
{
    public GameObject teleportText;
    GameObject player;

    float lastLook = 0;

    GameObject sceneLoader;

    public bool lastlevel;
    private void Start()
    {
        sceneLoader = GameObject.FindWithTag("levelLoader");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
       if(lastLook > Time.time)
        {
            teleportText.SetActive(true);
        }
        else
        {
            teleportText.SetActive(false);
        }
    }

    public void LookedAt()
    {
        lastLook = Time.time + 0.1f;
    }

    public void NextLevel()
    {
        if (!lastlevel)
        {

            sceneLoader.GetComponent<LevelLoader1>().LoadNextLevel();
            GameObject levelSaver = GameObject.FindGameObjectWithTag("saveManager");
            levelSaver.GetComponent<SaveManager>().Save();
            
        }
    }
}
