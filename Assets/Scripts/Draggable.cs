using UnityEngine;

public class Draggable : MonoBehaviour {

    public bool isAvailable { get; private set; }
    public bool isPizza;

    void Awake() {
        isAvailable = true;
        isPizza = GetComponent<Pizza>();
    }


    public void startDrag() {
        isAvailable = false;
    }

    public void endDrag() {
        isAvailable = true;
    }


}
