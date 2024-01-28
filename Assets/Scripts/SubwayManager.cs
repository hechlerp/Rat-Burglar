using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubwayManager : MonoBehaviour {

    public const string subwayPropName = "subway";
    class Track {
        public int idx;
        public Vector3 pos;
        public Vector3 dir;
        public float selectionWeight;
        public GameObject rails;
        public bool isExpress;
        public bool isAvailable { get; set; }
    }

    [SerializeField]
    Subway subwayPrefab;

    [SerializeField]
    Passenger passengerPrefab;

    //List<Subway> subways;
    float subwayTimer;
    float subwayInterval;

    public const string trackPosKey = "track";

    [SerializeField]
    List<Vector3> trackPositions;

    [SerializeField]
    List<Vector3> trackDirs;

    [SerializeField]
    List<float> baseTrackWeights;

    [SerializeField]
    List<float> thresholdWeights;

    [SerializeField]
    List<GameObject> rails;

    const int expressTrackIdx = 3;

    List<Track> tracks;

    private void Awake() {
        //subways = new List<Subway>();
        subwayInterval = 0;
        subwayTimer = -1;
        enabled = false;
    }



    public void initialize(float subwayInterval) {
        PropObjectPool.addPropPrefabToStore(subwayPropName, subwayPrefab.gameObject);
        PropObjectPool.addPropPrefabToStore(Passenger.passengerPrefabName, passengerPrefab.gameObject);
        enabled = true;
        this.subwayInterval = subwayInterval;
        subwayTimer = subwayInterval;
        tracks = new List<Track>();
        for (int i = 0; i < trackPositions.Count; i++) {
            tracks.Add(new Track() {
                idx = i,
                dir = trackDirs[i],
                pos = trackPositions[i],
                selectionWeight = baseTrackWeights[i],
                isAvailable = true,
                isExpress = i == expressTrackIdx,
                rails = rails[i]
            });
        }
    }

    private void FixedUpdate() {
        decrementSubwayTimer();
    }


    void decrementSubwayTimer() {
        subwayTimer -= Time.fixedDeltaTime;
        if (subwayTimer <= 0) {
            subwayTimer = subwayInterval;
            spawnSubway();
        }
    }

    public void spawnSubway() {
        float totalWeighting = 1;
        List<Track> availableTracks = tracks.Where(track => {
            if (!track.isAvailable) {
                totalWeighting -= track.selectionWeight;
            }
            return track.isAvailable;
        }).ToList();
        if (availableTracks.Count == 0 || totalWeighting == 0) {
            return;
        }
        List<float> adjustedWeights = availableTracks.Select(track => {
            return track.selectionWeight / totalWeighting;
        }).ToList();
        float randomVal = UnityEngine.Random.value;

        int trackIdx = 0;
        float weightTotal = 0;
        for (int i = 0; i < adjustedWeights.Count; i++) {
            weightTotal += adjustedWeights[i];
            if (randomVal <= weightTotal) {
                trackIdx = i;
                break;
            }
        }
        int trackIdxToUse = availableTracks[trackIdx].idx;
        Track track = tracks[trackIdxToUse];
        track.isAvailable = false;
        IPoolableProp subwayProp = PropObjectPool.getFirstAvailableProp(subwayPropName);
        Subway subway = (Subway)subwayProp;
        Action moveCompleteAction = () => { onMovementComplete(track.idx); };
        subway.activate(new Dictionary<string, object> {
            { Subway.dirKey, track.dir },
            { trackPosKey, track.pos },
            { Subway.movementCompleteCallbackKey, moveCompleteAction },
            { Subway.isExpressKey, trackIdxToUse == expressTrackIdx },
            { Subway.trackGoKey, track.rails.transform.GetChild(0).gameObject }
        });
    }

    void onMovementComplete(int trackIdx) {
        Track track = tracks[trackIdx];
        track.isAvailable = true;
    }

    public void toggleThresholdWeighting(bool shouldActivateExpress) {
        for (int i = 0; i < tracks.Count; i++) {
            tracks[i].selectionWeight = shouldActivateExpress ? thresholdWeights[i] : baseTrackWeights[i];
        }
    }

}
