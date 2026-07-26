using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference _move;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _playerController;
    private Vector3 _velocity;
    private Vector2 _moveDirection;

    private void Start()
    {
        _playerController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (_playerController.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _moveDirection = _move.action.ReadValue<Vector2>();
        Vector3 horizontalMove = transform.right * _moveDirection.x + transform.forward * _moveDirection.y;
        horizontalMove = Vector3.ClampMagnitude(horizontalMove, 1f);

        _velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = horizontalMove * moveSpeed + new Vector3(0f, _velocity.y, 0f);
        _playerController.Move(finalMove * Time.deltaTime);
    }
}