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

    void Awake() {
        movementCoroutine = null;
        movementCompleteCallback = null;
        enabled = false;
        passengerSpawnRate = stationaryTime / (maxPassengers + 1);
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
        movementCoroutine = StartCoroutine(moveIntoStation());
    }

    IEnumerator moveIntoStation() {
        float totalMovementTime = offsetMagnitude / (vInitial / 2);
        float accel = vInitial / totalMovementTime;
        float velocity = vInitial;
        WaitForFixedUpdate fixedUpdateDelay = new WaitForFixedUpdate();
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
        IPoolableProp spawnedPassenger = PropObjectPool.getFirstAvailableProp(Passenger.passengerPrefabName);
        spawnedPassenger.activate(new Dictionary<string, object>() {
            { Passenger.spawnPosKey, spawnPos },
            { Passenger.exitDirKey, Quaternion.AngleAxis(90, Vector3.forward) * dir }
        });
    }

}
