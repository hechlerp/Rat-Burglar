using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragTooltip : MonoBehaviour {
    public bool isAvailable { get; set; }

    public const string positionKey = "position";

    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Image background;

    private void Start() {
        deactivate();
    }

    public void activate() {
        text.enabled = true;
        background.enabled = true;
    }

    public void deactivate() {
        text.enabled = false;
        background.enabled = false;
    }
}
