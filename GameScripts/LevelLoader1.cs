using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader1 : MonoBehaviour
{
    public static LevelLoader1 instance;

    public GameObject loadScreen;
    GameObject levelCreator;

    public Slider progressbar;

    public GameObject seedManger;
    public GameObject saveManager;

    public GameObject currentLevelText;

    private bool loadingNextLevel = false;

    [HideInInspector]
    public int currentLevel;

    [HideInInspector]
    public int levelSeed;

    private bool startingNewGame;

    private void Awake()
    {
        instance = this;

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        currentLevel = 0;

    }

    private void Start()
    {
        loadingNextLevel = false;
        levelSeed = seedManger.GetComponent<SetLevelSeed>().seed;
    }
    List<AsyncOperation> scenesloading = new List<AsyncOperation>();

    public void LoadGame()
    {

        loadingNextLevel = false;
        loadScreen.gameObject.SetActive(true);

        if (SceneManager.GetSceneByBuildIndex(1).isLoaded)
        {
            scenesloading.Add(SceneManager.UnloadSceneAsync(1));
        }
        scenesloading.Add(SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive));
        scenesloading.Add(SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
        StartCoroutine(GetTotalProgress());
        StartCoroutine(LoadFallBack());

        currentLevel = 1;

     
        startingNewGame = true;
      
    }

    public void LoadFromSave()
    {
        loadingNextLevel = false;
        loadScreen.gameObject.SetActive(true);

        if (SceneManager.GetSceneByBuildIndex(1).isLoaded)
        {
            scenesloading.Add(SceneManager.UnloadSceneAsync(1));
        }
        scenesloading.Add(SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive));

            scenesloading.Add(SceneManager.LoadSceneAsync(currentLevel + 2, LoadSceneMode.Additive));

      

        StartCoroutine(GetSceneLoadProgress());
        StartCoroutine(GetTotalProgress());
        StartCoroutine(LoadFallBack());

        startingNewGame = false;
    }

    public void ReloadGame()
    {
        
        SceneManager.UnloadSceneAsync(currentLevel + 2);
        SceneManager.UnloadSceneAsync(2);
        LoadGame();

    }

    public void LoadMenu()
    {
        loadingNextLevel = false;
        scenesloading.Add(SceneManager.UnloadSceneAsync(currentLevel + 2));
        scenesloading.Add(SceneManager.UnloadSceneAsync(2));
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        currentLevel = 0;
        startingNewGame = false;
    }

    public void LoadNextLevel()
    {
        loadingNextLevel = true;
        saveManager.GetComponent<SaveManager>().Save();
        levelCreator = null;
        loadScreen.gameObject.SetActive(true);
        SceneManager.UnloadSceneAsync(2);
        SceneManager.UnloadSceneAsync(currentLevel + 2);
        currentLevel += 1;

        scenesloading.Add(SceneManager.LoadSceneAsync(currentLevel + 2, LoadSceneMode.Additive));
        scenesloading.Add(SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
        StartCoroutine(GetTotalProgress());
        startingNewGame = false;
    }
    float totalSceneProgress;
    float totalSpawnProgress;
    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesloading.Count; i++)
        {
            while (!scenesloading[i].isDone)
            {
                totalSceneProgress = 0;

                foreach(AsyncOperation operation in scenesloading)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress / scenesloading.Count) * 100f;
                
                
                yield return null;
            }
        }
        levelCreator = GameObject.FindWithTag("LevelCreator");
        levelCreator.GetComponent<LevelCreatorV2>().StartGeneration(levelSeed);

        if(SceneManager.GetSceneByBuildIndex(currentLevel + 2).isLoaded == true)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentLevel + 2));
        }

    }

  

    public IEnumerator GetTotalProgress()
    {
        float totalProgress = 0;

        while (levelCreator == null || !levelCreator.GetComponent<LevelCreatorV2>().isDone)
        {
            if(levelCreator == null)
            {
                totalSpawnProgress = 0;
            }
            else
            {
                totalSpawnProgress =Mathf.Round( levelCreator.GetComponent<LevelCreatorV2>().progress);
            }

            totalProgress = Mathf.Round((totalSceneProgress + totalSpawnProgress) / 2);
            progressbar.value = Mathf.RoundToInt(totalProgress);

            yield return null;

        }
        if(loadingNextLevel == true)
        {
            saveManager.GetComponent<SaveManager>().Load();
            currentLevel += 1;
        }
        
        loadScreen.gameObject.SetActive(false);
        currentLevelText.GetComponent<Animator>().SetTrigger("newLevel");
        currentLevelText.GetComponent<TextMeshProUGUI>().text = "Floor " + currentLevel;

        saveManager.GetComponent<SaveManager>().Save();

        if(startingNewGame == true)
        {
            this.GetComponent<SetPlayerDefaults>().SetDefaults();
        }
    }

    public IEnumerator LoadFallBack()
    {
        yield return new WaitForSeconds(3f);

            loadScreen.SetActive(false);
        
        
    }

}
