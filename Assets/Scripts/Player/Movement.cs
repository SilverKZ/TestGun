using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2.5f;
    [SerializeField] private float _runSpeed = 3.75f;
    [SerializeField] private float _rotationSpeed = 10f;

    private CharacterController _characterController;
    private Camera _camera;
    private Animator _animator;
    private PlayerInput _playerInput;
    private InputAction _lookAction;
    private InputAction _moveAction;
    private InputAction _runAction;
    private bool _isMove;

    private void Awake()
    {
        _camera = Camera.main;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
        _runAction = _playerInput.actions["Run"];
    }

    private void Update()
    {
        AimTowardMouse();
        Move();
    }

    private void Move()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(input.x, 0f, input.y);
        float runInput = _runAction.ReadValue<float>();
        _isMove = movement.magnitude > 0.1f;

        if (_isMove)
        {
            movement.Normalize();
            float speed = runInput * _runSpeed + _movementSpeed;
            movement *= speed * Time.deltaTime;
            _characterController.Move(movement);
        }

        PlayAnimaton(movement, runInput);
    }

    private void AimTowardMouse()
    {
        if (_isMove == false) return;

        Vector2 look = _lookAction.ReadValue<Vector2>();
        Ray ray = _camera.ScreenPointToRay(new Vector2(look.x, look.y));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void PlayAnimaton(Vector3 movement, float runInput)
    {
        float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
        float velocityX = Vector3.Dot(movement.normalized, transform.right);
        _animator.SetFloat("VelocityZ", runInput * velocityZ + velocityZ, 0.1f, Time.deltaTime);
        _animator.SetFloat("VelocityX", runInput * velocityX + velocityX, 0.1f, Time.deltaTime);
    }
}
