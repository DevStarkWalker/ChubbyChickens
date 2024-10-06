using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LevelBuilder LevelBuilder;
    public BirdController player;
    public TextMeshProUGUI countDownUI;
    public TextMeshProUGUI LevelUI;
    public GameObject restartScreen;
    public bool _nextLevel = false;
    public int level = 1;
    public GameObject lastTouchedPiece;
    public int playerCount = 1;

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
        if (Application.isFocused)
        {
            UnityEngine.Cursor.visible = true;
        }
    }

    public void FinishedLevel()
    {
        Debug.Log("FinishedLevel");
        LevelBuilder.BuildLevel();
    }


    public void OutOfBounds(GameObject go)
    {

    }


    public void GameOver()
    {
        timingGame = false;
        player.Died();
        player.animator.SetBool("IsDead",true);
        restartScreen.SetActive(true);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Debug.Log("You Suck");
    }

    public void RespawnPlayer()
    {
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }    
}
