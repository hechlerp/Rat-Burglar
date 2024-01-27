using UnityEngine;

public class SubwayCollider : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            LevelManager.loseGame();
        } else {
            Draggable draggable = other.GetComponent<Draggable>();
            if (draggable != null) {
                if (draggable.isPizza) {
                    LevelManager.resetPlayersAndPizza();
                } else {
                    if (draggable.isAvailable) {
                        Destroy(draggable);
                    } else {
                        PlayerController draggingPlayer = LevelManager.getPlayers().Find(player => player.getDraggedItem() == draggable);
                        draggingPlayer.dropIt();
                        Destroy(draggable);
                    }
                }
            }
        }
    }
}
