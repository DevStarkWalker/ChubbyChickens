using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BirdController player;
    public TextMeshProUGUI countDownUI;
    public TextMeshProUGUI LevelUI;
    public GameObject restartScreen;
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
        //DontDestroyOnLoad(gameObject);
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

    }

    public void FinishedLevel()
    {

    }


    public void OutOfBounds(GameObject go)
    {

    }


    public void GameOver()
    {
        timingGame = false;
        //player.canMove = false;
        player.animator.Play("Death");
        restartScreen.SetActive(true);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Debug.Log("You Suck");
    }

    private void UpdateSpawners()
    {

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

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }    
}
