using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    GameObject levelLoader;
    GameObject saveManager;
    GameObject seedManager;

    public GameObject mainMenu;
    public GameObject playMenu;

    private void Start()
    {
        mainMenu.SetActive(true);
        playMenu.SetActive(false);

        levelLoader = GameObject.FindGameObjectWithTag("levelLoader");
        saveManager = GameObject.FindGameObjectWithTag("saveManager");
        seedManager = GameObject.FindGameObjectWithTag("seedGen");
    }
    public void Play()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void NewGame()
    {
       // levelLoader.GetComponent<LevelLoader1>().levelSeed = seedManager.GetComponent<SetLevelSeed>().seed;
        levelLoader.GetComponent<LevelLoader1>().LoadGame();

    }

    public void LoadGame()
    {
        
        saveManager.GetComponent<SaveManager>().Load();

        StartCoroutine(LoadSaveGame());
    }

    IEnumerator LoadSaveGame()
    {
        yield return new WaitForSeconds(0.2f);
        levelLoader.GetComponent<LevelLoader1>().LoadFromSave();
       // levelLoader.GetComponent<LevelLoader1>().levelSeed = seedManager.GetComponent<SetLevelSeed>().seed;
    }
}
