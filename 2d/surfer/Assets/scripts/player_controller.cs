using UnityEngine;
using UnityEngine.InputSystem;

public class player_controller : MonoBehaviour
{
    Vector2 moveVector;
    [SerializeField] float torqueAmount = 1f;
    [SerializeField] float baseSpeed = 15f;
    [SerializeField] float boostSpeed = 20f;

    SurfaceEffector2D surfaceEffector2D;

    [SerializeField] bool canControlPlayer = true;

    InputAction moveAction;
    Rigidbody2D rb;

    void CalculateFlips() {
        float currentRotation = transform.rotation.eulerAngles.z;

        Debug.Log("Current rotation: " + currentRotation);

    }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();

        surfaceEffector2D = FindFirstObjectByType<SurfaceEffector2D>();
    }

    public void HandleCanControlPlayer(bool canControlPlayer) {
        this.canControlPlayer = canControlPlayer;
    }

    void Update()
    {
        if (canControlPlayer) {
            rotatePlayer();
            boostPlayer();
        }
        CalculateFlips();
    }

    void rotatePlayer() {
        moveVector = moveAction.ReadValue<Vector2>();
        rb.AddTorque(-moveVector.x * torqueAmount * Time.deltaTime); 
    }

    void boostPlayer() {
        if (moveVector.y > 0) {
            surfaceEffector2D.speed = boostSpeed;
        } else {
            surfaceEffector2D.speed = baseSpeed;
        }
    }
}
