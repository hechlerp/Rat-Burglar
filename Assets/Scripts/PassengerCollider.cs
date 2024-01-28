using UnityEngine;

public class PassengerCollider : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            LevelManager.loseGame();
        }
    }
}
