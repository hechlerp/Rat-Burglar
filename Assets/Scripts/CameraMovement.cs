using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    List<PlayerController> players;
    // Start is called before the first frame update
    void Start() {
        players = LevelManager.getPlayers();
    }

    // Update is called once per frame
    void Update() {
        Vector3 avgPos = Vector3.zero;
        foreach (PlayerController player in players) {
            avgPos += player.transform.position;
        }
        avgPos /= players.Count;
        transform.position = new Vector3(avgPos.x, avgPos.y, transform.position.z);
    }
}
