using UnityEngine;

public class DraggableCollider : MonoBehaviour {
    Draggable draggableParent;
    // Start is called before the first frame update
    void Awake() {
        draggableParent = transform.parent.GetComponent<Draggable>();
    }


    private void OnTriggerEnter2D(Collider2D other) {
        PlayerController pController = other.GetComponent<PlayerController>();
        if (pController != null) {
            pController.addPickupToRange(draggableParent);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        PlayerController pController = other.GetComponent<PlayerController>();
        if (pController != null) {
            pController.removePickupFromRange(draggableParent);
        }
    }
}
