using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragTooltip : MonoBehaviour, IPoolableProp {
    public bool isAvailable { get; set; }

    public const string positionKey = "position";

    [SerializeField]
    TextMeshPro tmPropRenderer;

    public void activate(Dictionary<string, object> args) {
        isAvailable = false;
        gameObject.SetActive(true);
        transform.position = (Vector3)args[positionKey];
    }

    public void deactivate() {
        gameObject.SetActive(false);
        isAvailable = true;
    }
}
