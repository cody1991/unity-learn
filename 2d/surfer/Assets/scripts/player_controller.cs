using UnityEngine;
using UnityEngine.InputSystem;

public class player_controller : MonoBehaviour
{
    [SerializeField] float torqueAmount = 1f;
    InputAction moveAction;

    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");

        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveVector;
        moveVector = moveAction.ReadValue<Vector2>();

        Debug.Log(moveVector);

        rb.AddTorque(-moveVector.x * torqueAmount); 
    }
}
