using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Controller player;
    public HillSpawn hillSpawner; 
    public List<ObjectSpawner> spawns;
    public TextMeshProUGUI countUI;
    public TextMeshProUGUI LevelUI;
    public bool _nextLevel = false;
    public int level = 1;

    public int difficulty = 1;
    public int length = 5;
    public int degree = 10;
    public float spawnRate = 1;
    public int spawnSpeed = 5;

    private int levelTicker = 0;
    private int difficultyTicker = 1;
    private float countDown = 4;
    private bool countDownTimer = false;
    private bool timingGame = false;
    private float gameTime;



    void Awake()
    {
        // Check if an instance of GameManager already exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy the duplicate instance
            return;
        }

        // Set this instance as the active instance
        Instance = this;

        // Optional: Make this GameManager persistent across scenes
        DontDestroyOnLoad(gameObject);
        UpdateSpawners();
    }
    // Start is called before the first frame update
    void Start()
    {
        countDownTimer = true;
        gameTime = 0;
        timingGame = true;
        LevelUI.text = "Level: 1";
    }

    // Update is called once per frame
    void Update()
    {

        if (_nextLevel)
        {
            hillSpawner.CreateLevel();
            _nextLevel = false;
            LevelUI.text = "Level: " + level;
        }
        if(timingGame) { gameTime += Time.deltaTime; }
        if (countDownTimer)
        {
            player.canMove = false;
            countUI.gameObject.SetActive(true);
            countDown -= Time.deltaTime;
            countUI.text = ((int)countDown).ToString();
            if (countDown < 0)
            {
                countUI.gameObject.SetActive(false);
                countDownTimer = false;
                countDown = 4;
                player.canMove = true;
                foreach (var t in spawns)
                {
                    t.enabled = true;
                }
            }
        }
    }

    public void FinishedLevel()
    {
        _nextLevel = true;
        foreach (var t in spawns)
        {
            t.FinishedLevel();
        }

        level++;
        levelTicker++;
        AdjustLevels();
        if(difficultyTicker == 5)
        {
            difficultyTicker = 0;
            difficulty++;
        }
        UpdateSpawners();
        countDownTimer = true;
    }


    public void OutOfBounds(GameObject go)
    {
        foreach (var t in spawns)
        {
            foreach (var t2 in t.spawnedObjects)
            {
                if (t2 == go)
                {
                    t.RemoveObject(go);
                    break;
                }
            }
        }    
    }


    public void GameOver()
    {
        timingGame = false;
        Debug.Log("You Suck");
    }

    private void UpdateSpawners()
    {
        hillSpawner.SetLevelAdjustments();
    }

    private void AdjustLevels()
    {
        switch (levelTicker) 
        {
            case 1:
                length += 5; break;
            case 2:
                degree += 5; break;
            case 3:
                spawnRate-= .1f;
                spawnSpeed ++;
                break;
        }
    }

    
}
