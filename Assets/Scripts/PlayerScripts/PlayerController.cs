using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;

        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}

// using UnityEngine;
// using UnityEngine.InputSystem;
//
// [RequireComponent(typeof(CharacterController))]
// public class PlayerController : MonoBehaviour
// {
//     [SerializeField] private InputActionReference _move;
//     [SerializeField] private float moveSpeed = 10f;
//     [SerializeField] private float gravity = -9.81f;
//
//     private CharacterController _playerController;
//     private Vector3 _velocity;
//     private Vector2 _moveDirection;
//
//     private void Start()
//     {
//         _playerController = GetComponent<CharacterController>();
//     }
//
//     private void Update()
//     {
//         if (_playerController.isGrounded && _velocity.y < 0)
//             _velocity.y = -2f;
//
//         _moveDirection = _move.action.ReadValue<Vector2>();
//         Vector3 horizontalMove = transform.right * _moveDirection.x + transform.forward * _moveDirection.y;
//         horizontalMove = Vector3.ClampMagnitude(horizontalMove, 1f);
//
//         _velocity.y += gravity * Time.deltaTime;
//
//         Vector3 finalMove = horizontalMove * moveSpeed + new Vector3(0f, _velocity.y, 0f);
//         _playerController.Move(finalMove * Time.deltaTime);
//     }
// }