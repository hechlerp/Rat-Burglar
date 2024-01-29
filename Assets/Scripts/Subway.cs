using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subway : MonoBehaviour, IPoolableProp {
    public bool isAvailable { get; set; }

    Coroutine movementCoroutine;
    Vector3 dir = Vector3.up;
    public const string dirKey = "dir";
    public const string movementCompleteCallbackKey = "onMovementComplete";

    [SerializeField]
    float offsetMagnitude;

    [SerializeField]
    float vInitial;

    [SerializeField]
    float stationaryTime;

    Vector3 endingPos;
    float movementTimer;
    float passengerSpawnTimer;
    Action movementCompleteCallback;

    int minPassengers = 3;
    int maxPassengers = 10;
    float passengerSpawnRate;

    [SerializeField]
    List<Vector3> localSpawnPositions;

    [SerializeField]
    float dismebarkYVariance;

    float railShakeMagnitude = .5f;
    float railShakeDur = 2f;
    GameObject trackGO;
    public const string trackGoKey = "trackGO";


    public const string isExpressKey = "isExpress";
    bool isExpress;

    void Awake() {
        movementCoroutine = null;
        movementCompleteCallback = null;
        enabled = false;
        passengerSpawnRate = stationaryTime / (maxPassengers + 1);
        isExpress = false;
        trackGO = null;
    }

    private void FixedUpdate() {
        movementTimer += Time.fixedDeltaTime;
        if (passengerSpawnTimer > -1) {
            passengerSpawnTimer += Time.fixedDeltaTime;
        }
    }

    public void activate(Dictionary<string, object> args) {
        isAvailable = false;
        dir = (Vector3)args[dirKey];
        endingPos = (Vector3)args[SubwayManager.trackPosKey];
        movementCompleteCallback = (Action)args[movementCompleteCallbackKey];
        transform.position = endingPos + (dir * offsetMagnitude * -1);
        gameObject.SetActive(true);
        enabled = true;
        movementTimer = 0;
        trackGO = (GameObject)args[trackGoKey];
        movementCoroutine = StartCoroutine(moveIntoStation());
        isExpress = (bool)args[isExpressKey];
    }

    IEnumerator moveIntoStation() {
        WaitForFixedUpdate fixedUpdateDelay = new WaitForFixedUpdate();
        bool lastVibratedLeft = true;
        while (movementTimer < railShakeDur) {
            // shake
            trackGO.transform.localPosition = (lastVibratedLeft ? Vector3.right : Vector3.left) * railShakeMagnitude;
            lastVibratedLeft = !lastVibratedLeft;
            yield return fixedUpdateDelay;
        }
        trackGO.transform.localPosition = Vector3.zero;

        movementTimer = 0;
        if (isExpress) {
            float totalTime = (offsetMagnitude * 2) / vInitial;
            while (movementTimer < totalTime) {
                transform.position += dir * vInitial * Time.fixedDeltaTime;
                yield return fixedUpdateDelay;
            }
            deactivate();
            yield break;
        }
        float totalMovementTime = offsetMagnitude / (vInitial / 2);
        float accel = vInitial / totalMovementTime;
        float velocity = vInitial;
        while (movementTimer < totalMovementTime) {
            transform.position += dir * velocity * Time.fixedDeltaTime;
            velocity -= accel * Time.fixedDeltaTime;
            yield return fixedUpdateDelay;
        }
        movementTimer = 0;
        passengerSpawnTimer = 0;
        int numPassengers = UnityEngine.Random.Range(minPassengers, maxPassengers);
        int spawnedPassengerCount = 0;
        while (movementTimer < stationaryTime) {
            if (spawnedPassengerCount >= numPassengers) {
                passengerSpawnTimer = -1;
            }
            if (passengerSpawnTimer > passengerSpawnRate) {
                passengerSpawnTimer = 0;
                spawnedPassengerCount++;
                spawnPassenger();
            }
            yield return fixedUpdateDelay;
        }
        movementTimer = 0;
        while (movementTimer < totalMovementTime) {
            transform.position += dir * velocity * Time.fixedDeltaTime;
            velocity += accel * Time.fixedDeltaTime;
            yield return fixedUpdateDelay;
        }
        deactivate();
    }

    public void deactivate() {
        enabled = false;
        gameObject.SetActive(false);
        isAvailable = true;
        if (movementCompleteCallback != null) {
            movementCompleteCallback();
        }
    }

    void spawnPassenger() {
        int spawnIdx = UnityEngine.Random.Range(0, localSpawnPositions.Count);
        Vector3 spawnPos = transform.TransformPoint(localSpawnPositions[spawnIdx]);
        spawnPos.y += (UnityEngine.Random.value * dismebarkYVariance) - (dismebarkYVariance / 2);
        IPoolableProp spawnedPassenger = PropObjectPool.getFirstAvailableProp(Passenger.passengerPrefabName);
        spawnedPassenger.activate(new Dictionary<string, object>() {
            { Passenger.spawnPosKey, spawnPos },
            { Passenger.exitDirKey, Quaternion.AngleAxis(-90, Vector3.forward) * dir }
        });
    }

}
