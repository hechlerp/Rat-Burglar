using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    const float rootTwo = 1.41f;

    [SerializeField]
    float speed = 1f;

    List<Draggable> draggablesInRange;

    enum FacingDir {
        left,
        right,
        up,
        down
    };

    enum PlayerAction {
        moveUp,
        moveDown,
        moveLeft,
        moveRight,
        grab,
        squeak
    }

    FacingDir currentFacingDir;
    Dictionary<FacingDir, Vector2> dirsToVectors;
    Draggable draggedItem;
    float dragDistance;
    CircleCollider2D playerCollider;

    [SerializeField]
    DragTooltip draggableTooltip;

    [SerializeField]
    Canvas HUDCanvas;

    [SerializeField]
    bool isPlayer1;

    Dictionary<PlayerAction, KeyCode> playerActionsToKeys;

    [SerializeField]
    float squeakRadius;

    [SerializeField]
    float squeakCooldown;
    float squeakTimer;

    [SerializeField]
    ParticleSystem squeakPS;

    float currentSpeed;
    float dragSlowFactor = .5f;



    void Awake() {
        draggablesInRange = new List<Draggable>();
        currentFacingDir = FacingDir.down;
        draggedItem = null;
        playerCollider = GetComponent<CircleCollider2D>();
        dirsToVectors = new Dictionary<FacingDir, Vector2>() {
            { FacingDir.left, Vector2.left },
            { FacingDir.right, Vector2.right },
            { FacingDir.up, Vector2.up},
            { FacingDir.down, Vector2.down },
        };
        playerActionsToKeys = new Dictionary<PlayerAction, KeyCode>() {
            { PlayerAction.moveUp, isPlayer1 ? KeyCode.W : KeyCode.UpArrow },
            { PlayerAction.moveDown, isPlayer1 ? KeyCode.S : KeyCode.DownArrow },
            { PlayerAction.moveLeft, isPlayer1 ? KeyCode.A : KeyCode.LeftArrow},
            { PlayerAction.moveRight, isPlayer1 ? KeyCode.D : KeyCode.RightArrow},
            { PlayerAction.grab, isPlayer1 ? KeyCode.G : KeyCode.Keypad1 },
            { PlayerAction.squeak, isPlayer1 ? KeyCode.H : KeyCode.Keypad0 },
        };
        currentSpeed = speed;

    }

    void FixedUpdate() {
        handleMovement();
        handleSqueakCooldown();
    }

    private void Update() {
        handleDragging();
        handleSqueak();
    }

    void handleMovement() {
        Vector2 movementDir = new Vector3();
        if (Input.GetKey(playerActionsToKeys[PlayerAction.moveUp])) {
            movementDir.y += 1;
        }
        if (Input.GetKey(playerActionsToKeys[PlayerAction.moveDown])) {
            movementDir.y -= 1;
        }
        if (Input.GetKey(playerActionsToKeys[PlayerAction.moveRight])) {
            movementDir.x += 1;
        }
        if (Input.GetKey(playerActionsToKeys[PlayerAction.moveLeft])) {
            movementDir.x -= 1;
        }
        Vector3 movementVector = movementDir * currentSpeed * Time.fixedDeltaTime;
        if (movementDir.x != 0 && movementDir.y != 0) {
            movementVector /= rootTwo;
        }
        if (movementVector != Vector3.zero) {
            transform.position += movementVector;
            FacingDir prevDir = currentFacingDir;
            currentFacingDir = getCurrentDir(movementDir);
            if (currentFacingDir != prevDir) {
                showTooltipIfNeeded();
            }
            if (draggedItem != null) {
                Vector3 dragOffset = movementDir * dragDistance;
                if (movementDir.x != 0 && movementDir.y != 0) {
                    dragOffset /= rootTwo;
                }
                draggedItem.transform.position = transform.position - dragOffset;
            }
        }
    }

    void handleSqueakCooldown() {
        if (squeakTimer > 0) {
            squeakTimer -= Time.fixedDeltaTime;
            if (squeakTimer < 0) {
                squeakTimer = 0;
            }
        }
    }

    FacingDir getCurrentDir(Vector3 movementDir) {
        if (
            (currentFacingDir == FacingDir.left && movementDir.x < 0) ||
            (currentFacingDir == FacingDir.right && movementDir.x > 0) ||
            (currentFacingDir == FacingDir.up && movementDir.y > 0) ||
            (currentFacingDir == FacingDir.down && movementDir.y < 0)
        ) {
            return currentFacingDir;
        }
        if (movementDir.x > 0) {
            return FacingDir.right;
        } else if (movementDir.x < 0) {
            return FacingDir.left;
        } else if (movementDir.y > 0) {
            return FacingDir.up;
        } else {
            return FacingDir.down;
        }
    }

    void handleDragging() {
        if (Input.GetKeyUp(playerActionsToKeys[PlayerAction.grab])) {
            if (draggedItem != null) {
                stopDragging();
            } else {
                enactDragInDir();
            }
        }
    }

    void handleSqueak() {
        if (Input.GetKeyUp(playerActionsToKeys[PlayerAction.squeak]) && draggedItem == null && squeakTimer == 0) {
            squeak();
        }
    }

    void squeak() {
        squeakPS.Play();
        squeakTimer = squeakCooldown;
        Collider2D[] passengerCols = Physics2D.OverlapCircleAll(transform.position, squeakRadius, LayerMask.GetMask("Person"));
        foreach (Collider2D col in passengerCols) {
            Passenger passenger = col.GetComponent<Passenger>();
            passenger.panic(transform.position);
        }
    }

    void enactDragInDir() {
        Draggable bestDraggable = getTopDraggable();
        if (bestDraggable == null) {
            return;
        }
        draggedItem = bestDraggable;
        bestDraggable.startDrag();
        draggableTooltip.deactivate();
        dragDistance = playerCollider.radius + draggedItem.GetComponent<CircleCollider2D>().radius;
        currentSpeed = speed * dragSlowFactor;
    }

    Draggable getTopDraggable() {
        if (draggablesInRange.Count == 0) {
            return null;
        }
        List<(Draggable, float)> eligibleDraggables = draggablesInRange
            .Select(draggable => {
                return (draggable, Vector2.Dot(draggable.transform.position - transform.position, dirsToVectors[currentFacingDir]));
            })
            .Where(draggableTuple => {
                (Draggable draggable, float dotProduct) = draggableTuple;
                return draggable.isAvailable && dotProduct > 0;
            })
            .ToList();
        if (eligibleDraggables.Count == 0) {
            return null;
        }
        eligibleDraggables.Sort((left, right) => {
            (Draggable draggableA, float dotProductA) = left;
            (Draggable draggableB, float dotProductB) = right;
            return dotProductA > dotProductB ? 1 : -1;
        });
        return eligibleDraggables.First().Item1;
    }

    void stopDragging() {
        draggedItem.endDrag();
        draggedItem = null;
        showTooltipIfNeeded();
        currentSpeed = speed;
    }

    public void addPickupToRange(Draggable draggableObj) {
        draggablesInRange.Add(draggableObj);
        showTooltipIfNeeded();
    }

    public Draggable getDraggedItem() {
        return draggedItem;
    }

    public void dropIt() {
        draggedItem.endDrag();
        draggedItem = null;
        currentSpeed = speed;
    }

    void showTooltipIfNeeded() {
        Draggable bestDraggable = getTopDraggable();
        if (bestDraggable != null) {
            RectTransform tooltipRT = (draggableTooltip.transform as RectTransform);
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(bestDraggable.transform.position) / HUDCanvas.scaleFactor;
            screenPoint = new Vector3(screenPoint.x - (tooltipRT.sizeDelta.x / 2), (screenPoint.y - tooltipRT.sizeDelta.y / 2), screenPoint.z);
            tooltipRT.anchoredPosition = screenPoint;
            draggableTooltip.activate();
        } else {
            draggableTooltip.deactivate();
        }

    }

    public void removePickupFromRange(Draggable draggableObj) {
        draggablesInRange.Remove(draggableObj);
        draggableTooltip.deactivate();
        showTooltipIfNeeded();
    }


}
