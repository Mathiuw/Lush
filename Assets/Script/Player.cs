using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Input class
    private InputActions_Player inputActions;

    // Player movement variables
    [Header("Player Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float gravity = -9.81f;
    CharacterController characterController;
    Vector2 MoveInput = Vector2.zero;
    Vector3 MoveDirection = Vector3.zero;
    Vector3 gravityVelocity = Vector3.zero;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float sphereRadius = 0.2f;
    [SerializeField] float sphereOffset = 0f;

    // Camera movement variables
    [Header("Camera Movement")]
    [SerializeField] float sensibility = 20f;
    [SerializeField] Transform headPivot;
    Transform playerCamera;
    Vector2 lookInput = Vector2.zero;
    float xlookRotation = 0f;

    [Header("Head Bob")]
    [SerializeField] bool headBob = true;
    [SerializeField] float amplitude = 0.1f; 
    [SerializeField] float frequency = 2f;
    float bobTime = 0f;

    private void Awake()
    {
        // Lock Cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Get camera transform
        playerCamera = GetComponentInChildren<Camera>().transform;

        // Get character controller component
        characterController = GetComponent<CharacterController>();

        if (!characterController)
        {
            Debug.LogError("Cant find character controller");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        // Create input class
        inputActions = new InputActions_Player();

        // Add event
        // Movement actions
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        // Look actions
        inputActions.Player.Look.performed += OnLookPerformed;
        inputActions.Player.Look.canceled += OnLookCanceled;
        // Exit game action
        inputActions.Player.Exit.started += OnExitStarted;

        inputActions.Enable();
    }

    private void OnDisable()
    {
        // Remove event
        // Movement actions
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        // Look actions
        inputActions.Player.Look.performed -= OnLookPerformed;
        inputActions.Player.Look.canceled -= OnLookCanceled;
        // Exit game action
        inputActions.Player.Exit.started -= OnExitStarted;

        inputActions.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext callbackContext)
    {
        MoveInput = callbackContext.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext callbackContext) 
    {
        MoveInput = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext callbackContext) 
    {
        lookInput = callbackContext.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext callbackContext) 
    {
        lookInput = Vector2.zero;
    }

    private void OnExitStarted(InputAction.CallbackContext callbackContext) 
    {
        Application.Quit();
    }

    private void Update()
    {
        CameraMovement();
        PlayerMovement();

        // Head bob
        if (headBob)
        {
            Vector3 velocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);

            bobTime += velocity.magnitude * Time.deltaTime;
            Debug.Log(bobTime);

            playerCamera.localPosition = HeadBob(bobTime);
        }
    }

    private void PlayerMovement() 
    {
        // Gravity
        if (!Physics.CheckSphere(transform.position + Vector3.down + Vector3.up * sphereOffset, sphereRadius, groundMask))
        {
            gravityVelocity.y += gravity * Time.deltaTime;
        }
        else
        {
            gravityVelocity.y = -2;
        }

        characterController.Move(gravityVelocity * Time.deltaTime);

        // Movement
        MoveDirection = transform.forward * MoveInput.y + transform.right * MoveInput.x;

        characterController.Move(MoveDirection.normalized * speed * Time.deltaTime);
    }

    private void CameraMovement()
    {
        float mouseX = lookInput.x * sensibility * Time.deltaTime;
        float mouseY = lookInput.y * sensibility * Time.deltaTime;

        xlookRotation -= mouseY;

        // Clamp x rotation value
        xlookRotation = Math.Clamp(xlookRotation, -89, 89);

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.localRotation = Quaternion.Euler(xlookRotation, 0, 0);
    }

    private Vector3 HeadBob(float time)
    {
        Vector3 position = Vector3.zero;
        position.y += Mathf.Sin(time * frequency) * amplitude;
        //position.x += Mathf.Cos(time * frequency / 2) * amplitude;
        return position;
    }

    // DEBUG gizmos draw
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.down + Vector3.up * sphereOffset, sphereRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + MoveDirection.x, transform.position.y, transform.position.z + MoveDirection.z));
    }
}