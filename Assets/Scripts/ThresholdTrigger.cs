using UnityEngine;

public class ThresholdTrigger : MonoBehaviour {
    [SerializeField]
    bool shouldActivateExpress;

    private void OnTriggerEnter2D(Collider2D other) {
        Draggable draggable = other.GetComponent<Draggable>();
        if (draggable != null) {
            if (draggable.isPizza) {
                LevelManager.toggleSubwayWeights(shouldActivateExpress);
            }
        }
    }
}
