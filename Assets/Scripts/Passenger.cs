using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour, IPoolableProp {
    public bool isAvailable { get; set; }
    public const string spawnPosKey = "spawnPos";
    public const string exitDirKey = "exitDir";

    public const string passengerPrefabName = "passenger";
    const int numXDests = 6;
    const float destYMag = 30;
    const float destNegYMag = -40;

    Vector3 currentDir;

    float platformRaycastDist = 20f;
    float immediateVicinityDist = .25f;
    int platformBarrierLayer;
    int barrierAndObstacleLayer;
    Vector3 dest;
    float buffer = 1;

    [SerializeField]
    float speed;

    Coroutine movingCoroutine;

    float raycastTimer;
    const float raycastInterval = .25f;

    // Start is called before the first frame update
    void Awake() {
        platformBarrierLayer = LayerMask.GetMask("PersonBlocker");
        barrierAndObstacleLayer = LayerMask.GetMask(new string[2] {
            "PersonBlocker",
            "Obstacle"
        });
        raycastTimer = 0;
    }

    private void FixedUpdate() {
        raycastTimer += Time.fixedDeltaTime;
    }

    public void activate(Dictionary<string, object> args) {
        isAvailable = false;
        gameObject.SetActive(true);
        transform.position = (Vector3)args[spawnPosKey];
        currentDir = (Vector3)args[exitDirKey];
        enabled = true;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, currentDir, platformRaycastDist, platformBarrierLayer);
        if (hits.Length > 1) {
            float firstColWidth = ((BoxCollider2D)hits[0].collider).size.x / 2;
            float secondColWidth = ((BoxCollider2D)hits[1].collider).size.x / 2;
            float platformWidth = Vector3.Distance(hits[0].transform.position, hits[1].transform.position) - firstColWidth - secondColWidth;
            Vector3 platformStart = hits[0].transform.position + (firstColWidth * currentDir);
            Vector3 destDir = Mathf.FloorToInt(Random.value * 2) == 0 ? Vector3.up : Vector3.down;
            int xDestMultiplier = Random.Range(1, numXDests + 1);
            float destX = ((platformWidth - buffer) / numXDests * xDestMultiplier) + buffer;
            dest = new Vector3(destX * currentDir.x + platformStart.x, destDir.y == 1 ? destYMag : destNegYMag);
            raycastTimer = 0;
            movingCoroutine = StartCoroutine(move());
            return;
            //Debug.Log($"dest: {dest}");
            //Debug.Log(hits[0].transform.name);
        }
        Debug.Log("didn't find platforms. Check destination calc function");
        deactivate();
        //float platformWidth = platformRaycastDist;
    }

    public void deactivate() {
        gameObject.SetActive(false);
        enabled = false;
        isAvailable = true;
        raycastTimer = 0;
    }

    IEnumerator move() {
        WaitForFixedUpdate fixedUpdateDelay = new WaitForFixedUpdate();
        bool hasReachedRequisiteX = false;
        while (dest.y > 0 ? transform.position.y < dest.y : transform.position.y > dest.y) {
            if (!hasReachedRequisiteX) {
                if (dest.x > 0 ? transform.position.x < dest.x : transform.position.x > dest.x) {
                    transform.position += currentDir * speed * Time.fixedDeltaTime;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDir, immediateVicinityDist, barrierAndObstacleLayer);
                    if (hit.collider != null) {
                        hasReachedRequisiteX = true;
                        currentDir = getDestYDir();
                    }
                    yield return fixedUpdateDelay;
                } else {
                    hasReachedRequisiteX = true;
                    currentDir = getDestYDir();
                    yield return fixedUpdateDelay;
                }
            }
            transform.position += currentDir * speed * Time.fixedDeltaTime;
            if (raycastTimer > raycastInterval) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDir, immediateVicinityDist, barrierAndObstacleLayer);
                if (hit.collider != null) {
                    if (currentDir == getDestYDir()) {

                    }
                }

            }
            yield return fixedUpdateDelay;
        }

    }

    Vector3 getDestYDir() {
        return new Vector3(0, Mathf.Sign(dest.y), 0);
    }

    Vector3 getDestXDir() {
        return new Vector3(0, dest.x, transform.position.x);
    }

    private void OnDrawGizmos() {
        if (Application.isPlaying) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(dest, 1);
            Gizmos.DrawLine(transform.position, dest);
        }
    }
}
