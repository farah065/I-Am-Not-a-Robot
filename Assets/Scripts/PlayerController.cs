using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CinemachineCamera _cineCam;

    private InputAction _moveAction;
    private Vector2 _moveInput;

    private void Awake()
    {
        _moveAction = _inputActions.FindActionMap("Player").FindAction("Move");

        _moveAction.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _moveAction.canceled += ctx => _moveInput = Vector2.zero;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        _moveAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 forward = _cineCam.transform.forward;
        Vector3 right = _cineCam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 desiredMove = forward * _moveInput.y + right * _moveInput.x;
        desiredMove.Normalize();

        Vector3 velocity = desiredMove * _moveSpeed;
        velocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = velocity;

        if (desiredMove.sqrMagnitude > 0f)
        {
            Quaternion lookRot = Quaternion.LookRotation(desiredMove);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, lookRot, Time.fixedDeltaTime * 10f));
        }
    }
}
