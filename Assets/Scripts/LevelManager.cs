using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour {
    static LevelManager manager;
    public static LevelManager instance {
        get {
            if (manager == null) {
                manager = FindObjectOfType(typeof(LevelManager)) as LevelManager;
                if (manager == null) {
                    manager = new GameObject(typeof(LevelManager).ToString()).AddComponent<LevelManager>();
                }
                manager.initialize();
            }
            return manager;
        }
    }

    [SerializeField]
    SubwayManager subwayManager;

    float levelTimer;

    [SerializeField]
    float maxLevelTime;

    float subwayTimer;

    [SerializeField]
    float subwayInterval;

    int lastDisplayedSeconds;

    bool timerIsRunning;

    bool isRunning;
    bool isInitialized = false;

    List<PlayerController> players;
    List<Vector3> initialPlayerPositions;

    [SerializeField]
    TextMeshProUGUI timeText;

    [SerializeField]
    Pizza pizza;

    Vector3 initialPizzaPos;

    void initialize() {
        if (isInitialized) {
            return;
        }
        levelTimer = maxLevelTime;
        subwayTimer = subwayInterval;
        isRunning = true;
        timerIsRunning = true;
        lastDisplayedSeconds = -1;
        subwayManager.initialize(subwayInterval);
        GameObject[] playerGOs = GameObject.FindGameObjectsWithTag("Player");
        initialPlayerPositions = new List<Vector3>();
        players = new List<PlayerController>();
        foreach (GameObject player in playerGOs) {
            players.Add(player.GetComponent<PlayerController>());
            initialPlayerPositions.Add(new Vector3(player.transform.position.x, player.transform.position.y));
        }
        initialPizzaPos = new Vector3(pizza.transform.position.x, pizza.transform.position.y);
        isInitialized = true;
    }

    void Start() {
        initialize();
    }

    void FixedUpdate() {
        if (!isRunning) {
            return;
        }
        decrementLevelTimer();
    }

    void decrementLevelTimer() {
        if (timerIsRunning)
        {
            if (levelTimer >= 0)
            {
                levelTimer -= Time.fixedDeltaTime;
                int currentSeconds = Mathf.FloorToInt(levelTimer) % 60;
                if(currentSeconds != lastDisplayedSeconds)
                {
                    displayLevelTimer(levelTimer);
                    lastDisplayedSeconds = currentSeconds;
                }
            }
            else
            {
                Debug.Log("Counted down to zero");
                loseGame();
            }
        }
    }

    void displayLevelTimer(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public static void winGame() {
        instance.isRunning = false;
        instance.enabled = false;
        instance.levelTimer = 0;
        instance.timerIsRunning = false;
        Debug.Log("players won!");
    }

    public static void loseGame() {
        instance.isRunning = false;
        instance.enabled = false;
        instance.levelTimer = 0;
        instance.timerIsRunning = false;
        Debug.Log("players lost!");
    }

    public static void resetPlayersAndPizza() {
        for (int i = 0; i < instance.players.Count; i++) {
            instance.players[i].transform.position = instance.initialPlayerPositions[i];
        }
        instance.pizza.transform.position = instance.initialPizzaPos;
    }

    public static List<PlayerController> getPlayers() {
        return instance.players;
    }
}
