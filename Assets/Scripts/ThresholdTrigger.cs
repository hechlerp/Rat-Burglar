using UnityEngine;

public class ThresholdTrigger : MonoBehaviour {
    [SerializeField]
    bool shouldActivateExpress;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            LevelManager.toggleSubwayWeights(shouldActivateExpress);
        }
    }
}
