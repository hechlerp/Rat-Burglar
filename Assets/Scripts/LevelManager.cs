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

    float levelTimer;

    [SerializeField]
    float maxLevelTime;

    float subwayTimer;

    [SerializeField]
    float subwayInterval;

    bool isRunning;

    void initialize() {
        levelTimer = maxLevelTime;
        subwayTimer = subwayInterval;
        isRunning = true;
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {

    }

    public static void winGame() {
        instance.isRunning = false;
        instance.enabled = false;
    }

    public static void loseGame() {
        instance.isRunning = false;
        instance.enabled = false;
    }
}
