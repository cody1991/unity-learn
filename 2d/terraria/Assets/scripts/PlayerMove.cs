using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{


    Vector2 moveInput;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }
}
