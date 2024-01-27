using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour, IPoolableProp {
    public bool isAvailable { get; set; }
    public const string spawnPosKey = "spawnPos";
    public const string exitDirKey = "exitDir";

    public const string passengerPrefabName = "passenger";

    Vector3 startingDir;

    [SerializeField]
    float speed;

    // Start is called before the first frame update
    void Start() {

    }

    private void FixedUpdate() {
        transform.position += startingDir * speed * Time.fixedDeltaTime;
    }

    public void activate(Dictionary<string, object> args) {
        isAvailable = false;
        gameObject.SetActive(true);
        transform.position = (Vector3)args[spawnPosKey];
        startingDir = (Vector3)args[exitDirKey];
        enabled = true;
    }

    public void deactivate() {
        gameObject.SetActive(false);
        enabled = false;
        isAvailable = true;
    }
}
