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
    float panicTimer;
    bool isPanicking;

    [SerializeField]
    float panicDuration;

    [SerializeField]
    float panicSpeed;

    const float raycastInterval = .25f;
    bool isTravelingInBadDir;
    //bool hasGottenPastFirstBarrier;
    Collider2D firstBarrier;
    bool hasReachedRequisiteX;

    [SerializeField]
    List<Sprite> walkFAnim;

    [SerializeField]
    List<Sprite> walkBAnim;

    [SerializeField]
    List<Sprite> walkLAnim;

    [SerializeField]
    List<Sprite> walkRAnim;

    [SerializeField]
    List<Sprite> panicAnim;

    // Start is called before the first frame update
    void Awake() {
        platformBarrierLayer = LayerMask.GetMask("PersonBlocker");
        barrierAndObstacleLayer = LayerMask.GetMask(new string[2] {
            "PersonBlocker",
            "Obstacle"
        });
        raycastTimer = 0;
        panicTimer = 0;
        isPanicking = false;
        hasReachedRequisiteX = false;

    }

    private void FixedUpdate() {
        raycastTimer += Time.fixedDeltaTime;
        if (panicTimer > 0) {
            panicTimer -= Time.fixedDeltaTime;
            if (panicTimer <= 0) {
                endPanic();
            }
        }
    }

    public void activate(Dictionary<string, object> args) {
        isAvailable = false;
        gameObject.SetActive(true);
        transform.position = (Vector3)args[spawnPosKey];
        currentDir = (Vector3)args[exitDirKey];
        enabled = true;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, currentDir, platformRaycastDist, platformBarrierLayer);
        if (hits.Length > 1) {
            firstBarrier = hits[0].collider;
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
        hasReachedRequisiteX = false;
        Vector3 positionInitial = transform.position;
        while (dest.y > 0 ? transform.position.y < dest.y : transform.position.y > dest.y) {
            // if panicking, run directly away, into the nearest wall and chill.
            if (isPanicking) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDir, immediateVicinityDist, barrierAndObstacleLayer);
                if (hit.collider == null) {
                    transform.position += currentDir * speed * Time.fixedDeltaTime * panicSpeed;
                }
                yield return fixedUpdateDelay;
                continue;
            }
            if (!hasReachedRequisiteX) {
                if (dest.x > positionInitial.x ? transform.position.x < dest.x : transform.position.x > dest.x) {
                    transform.position += currentDir * speed * Time.fixedDeltaTime;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDir, immediateVicinityDist, barrierAndObstacleLayer);
                    // ignore the first barrier if it's our initial motion. We should get past that the first time to make it on to the platform.
                    if (hit.collider != null && hit.collider != firstBarrier) {
                        hasReachedRequisiteX = true;
                        currentDir = getDestYDir();
                    }
                    yield return fixedUpdateDelay;
                } else {
                    hasReachedRequisiteX = true;
                    currentDir = getDestYDir();
                    yield return fixedUpdateDelay;
                }
                continue;
            }
            transform.position += currentDir * speed * Time.fixedDeltaTime;
            if (raycastTimer > raycastInterval) {
                bool didChangeDir = false;
                if (isTravelingInBadDir) {
                    if (currentDir.y != 0) {
                        didChangeDir = tryAndMoveInXDirection();
                    } else {
                        didChangeDir = tryAndMoveInYDirection();
                    }
                }
                if (!didChangeDir) {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDir, immediateVicinityDist, barrierAndObstacleLayer);
                    if (hit.collider != null) {
                        findNextDirection();
                    }
                }

            }
            yield return fixedUpdateDelay;
        }
        deactivate();
    }

    void findNextDirection() {
        isTravelingInBadDir = false;
        Vector3 idealYDir = getDestYDir();
        if (canTravelInDir(idealYDir)) {
            currentDir = idealYDir;
            return;

        }
        Vector3 idealXDir = getDestXDir();
        if (idealXDir == Vector3.zero) {
            Vector3 arbitraryX = getArbitraryXDir();
            if (canTravelInDir(arbitraryX)) {
                currentDir = arbitraryX;
                isTravelingInBadDir = true;
                return;
            } else if (canTravelInDir(-arbitraryX)) {
                currentDir = -arbitraryX;
                isTravelingInBadDir = true;
                return;
            }
        }
        if (canTravelInDir(idealXDir)) {
            currentDir = new Vector3(Mathf.Sign(idealXDir.x), 0, 0);
            return;
        } else if (canTravelInDir(getBadXDir(idealXDir))) {
            currentDir = new Vector3(Mathf.Sign(getBadXDir(idealXDir).x), 0, 0);
            isTravelingInBadDir = true;
            return;
        }
        currentDir = getBadYDir(idealYDir);
        isTravelingInBadDir = true;
    }

    bool tryAndMoveInXDirection() {
        Vector3 idealXDir = getDestXDir();
        if (idealXDir == Vector3.zero) {
            Vector3 arbitraryX = getArbitraryXDir();
            if (canTravelInDir(arbitraryX)) {
                currentDir = arbitraryX;
                isTravelingInBadDir = true;
                return true;
            } else if (canTravelInDir(-arbitraryX)) {
                currentDir = -arbitraryX;
                isTravelingInBadDir = true;
                return true;
            }
        }
        if (canTravelInDir(idealXDir)) {
            currentDir = new Vector3(Mathf.Sign(idealXDir.x), 0, 0);
            return true;
        }
        return false;
    }

    bool tryAndMoveInYDirection() {
        Vector3 idealYDir = getDestYDir();
        if (canTravelInDir(idealYDir)) {
            currentDir = idealYDir;
            return true;
        }
        return false;
    }

    bool canTravelInDir(Vector3 dir) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDir, immediateVicinityDist, barrierAndObstacleLayer);
        return hit.collider == null;
    }

    Vector3 getDestYDir() {
        return new Vector3(0, Mathf.Sign(dest.y), 0);
    }

    Vector3 getDestXDir() {
        return new Vector3(dest.x - transform.position.x, 0);
    }

    Vector3 getBadYDir(Vector3 destYDir) {
        return -destYDir;
    }

    Vector3 getBadXDir(Vector3 destXDir) {
        return -destXDir;
    }

    Vector3 getArbitraryXDir() {
        return Mathf.FloorToInt(Random.value * 2) == 0 ? Vector3.left : Vector3.right;
    }

    public void panic(Vector3 ratPos) {
        // run in the opposite direction
        currentDir = (transform.position - ratPos).normalized;
        panicTimer = panicDuration;
        isPanicking = true;
    }

    // this seems to sometimes lead to shaking behavior and somewhat odd directions. Not sure why that is. Pretty rare bug tho.
    void endPanic() {
        isPanicking = false;
        currentDir = !hasReachedRequisiteX ? new Vector3(Mathf.Sign(getDestXDir().x), 0, 0) : getDestYDir();
    }

    // Uncomment below for debugging destinations etc.
    //private void OnDrawGizmos() {
    //    if (Application.isPlaying) {
    //        Gizmos.color = Color.blue;
    //        Vector3 immediateVicinityPt = transform.position + (immediateVicinityDist * currentDir);
    //        Gizmos.DrawSphere(immediateVicinityPt, 1);
    //        Gizmos.DrawLine(transform.position, immediateVicinityPt);
    //        Gizmos.DrawLine(transform.position, dest);
    //    }
    //}
}
