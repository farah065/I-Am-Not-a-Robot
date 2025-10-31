using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private CinemachineCamera _cineCam;

    [SerializeField] private float _forwardOffset = 1;
    [SerializeField] private float _verticalOffset = 1;
    [SerializeField] private float _xSize = 1;
    [SerializeField] private float _ySize = 1;
    [SerializeField] private float _zSize = 1;


    private InputAction _moveAction;
    private InputAction _interactAction;
    private Vector2 _moveInput;

    private void Awake()
    {
        _moveAction = _inputActions.FindActionMap("Player").FindAction("Move");
        _interactAction = _inputActions.FindActionMap("Player").FindAction("Interact");

        _moveAction.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _moveAction.canceled += ctx => _moveInput = Vector2.zero;

        _interactAction.performed += HandleInteract;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        _moveAction.Enable();
        _interactAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _interactAction.Disable();
    }

    private void OnDrawGizmos()
    {
        if (_cineCam == null) return;

        Gizmos.color = Color.green;

        Vector3 camForward = _cineCam.transform.forward;
        camForward.y = 0f; // optional — keeps box level with ground
        camForward.Normalize();

        Vector3 boxCentre = transform.position + camForward * _forwardOffset + Vector3.up * _verticalOffset;
        Vector3 boxSize = new Vector3(_xSize, _ySize, _zSize);

        // Draw box rotated in the direction the camera is facing
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCentre, Quaternion.LookRotation(camForward), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);

        // Reset gizmo matrix
        Gizmos.matrix = Matrix4x4.identity;
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

    private void HandleInteract(InputAction.CallbackContext context)
    {
        Vector3 camForward = _cineCam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 boxCenter = transform.position + camForward * _forwardOffset + Vector3.up * _verticalOffset;
        Vector3 boxHalfExtents = new Vector3(_xSize / 2, _ySize / 2, _zSize / 2);

        Debug.Log("INTERACT");

        // Get all colliders overlapping this oriented box
        Collider[] hits = Physics.OverlapBox(boxCenter, boxHalfExtents, Quaternion.LookRotation(camForward));

        if (hits.Length > 0)
        {
            Debug.Log($"Found {hits.Length} colliders in box.");
            foreach (Collider hit in hits)
            {
                IInteractable interactable = hit.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    Debug.Log($"Interacting with {hit.name}");
                    interactable.Interact();
                    break; // interact with first valid target
                }
            }
        }
        else
        {
            Debug.Log("No interactables found.");
        }
    }
}
