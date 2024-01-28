using System.Collections.Generic;
using UnityEngine;

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

    bool isRunning;
    bool isInitialized = false;

    List<PlayerController> players;
    List<Vector3> initialPlayerPositions;

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
        AudioManager.Instance.PlayAmbience();
    }

    void FixedUpdate() {
        if (!isRunning) {
            return;
        }
        decrementLevelTimer();
    }

    void decrementLevelTimer() {
        levelTimer -= Time.fixedDeltaTime;
        if (levelTimer <= 0) {
            loseGame();
        }
    }

    public static void winGame() {
        instance.isRunning = false;
        instance.enabled = false;
        Debug.Log("players won!");

    }

    public static void loseGame() {
        instance.isRunning = false;
        instance.enabled = false;
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
