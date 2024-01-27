using UnityEngine;

public class Draggable : MonoBehaviour {

    public bool isAvailable { get; private set; }

    void Awake() {
        isAvailable = true;
    }


    public void startDrag() {
        isAvailable = false;
    }

    public void endDrag() {
        isAvailable = true;
    }


}
