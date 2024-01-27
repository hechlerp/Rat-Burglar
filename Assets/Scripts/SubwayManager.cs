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
                isAvailable = true
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
        List<Track> availableTracks = tracks.Where(track => track.isAvailable).ToList();
        if (availableTracks.Count == 0) {
            return;
        }
        int trackIdx = UnityEngine.Random.Range(0, availableTracks.Count);
        int trackIdxToUse = availableTracks[trackIdx].idx;
        Track track = tracks[trackIdxToUse];
        track.isAvailable = false;
        IPoolableProp subwayProp = PropObjectPool.getFirstAvailableProp(subwayPropName);
        Subway subway = (Subway)subwayProp;
        Action moveCompleteAction = () => { onMovementComplete(track.idx); };
        subway.activate(new Dictionary<string, object> {
            { Subway.dirKey, track.dir },
            { trackPosKey, track.pos },
            { Subway.movementCompleteCallbackKey, moveCompleteAction }

        });
    }

    void onMovementComplete(int trackIdx) {
        Track track = tracks[trackIdx];
        track.isAvailable = true;
    }

}
