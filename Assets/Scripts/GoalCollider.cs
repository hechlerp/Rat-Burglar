using UnityEngine;

public class GoalCollider : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player")) {
            PlayerController player = other.GetComponent<PlayerController>();
            Draggable draggedItem = player.getDraggedItem();
            if (draggedItem != null && draggedItem.isPizza) {
                LevelManager.winGame();
            }
        }
    }
}
