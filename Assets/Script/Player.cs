using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Input class
    private InputActions_Player inputActions;

    // Player movement variables
    [Header("Player Movement")]
    [SerializeField] float moveSpeed = 5f;
    CharacterController characterController;
    Vector2 MoveInput = Vector2.zero;
    Vector3 MoveDirection = Vector3.zero;

    // Camera movement variables
    [Header("Camera Movement")]
    [SerializeField] float mouseSensibility = 5f;
    [SerializeField] Transform headPivot;
    Transform playerCamera;
    Vector2 lookInput = Vector2.zero;
    Vector2 lookRotation = Vector2.zero;

    private void Awake()
    {
        // Lock Cursor
        Cursor.visible = false;
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
    }

    private void PlayerMovement() 
    {
        MoveDirection = playerCamera.forward * MoveInput.y + playerCamera.right * MoveInput.x;

        // Gravity
        if (!characterController.isGrounded)
        {
            MoveDirection.y += Physics.gravity.y;
        }
        else if (characterController.isGrounded && MoveDirection.y != 0)
        {
            MoveDirection.y = 0;
        }

        characterController.Move(MoveDirection.normalized * moveSpeed * Time.deltaTime);
    }

    private void CameraMovement()
    {
        float mouseX = lookInput.x * mouseSensibility;
        float mouseY = lookInput.y * mouseSensibility;

        lookRotation.x -= mouseY;
        lookRotation.y = playerCamera.rotation.eulerAngles.y + mouseX;

        // Clamp x rotation value
        lookRotation.x = Math.Clamp(lookRotation.x, -89, 89);

        playerCamera.rotation = Quaternion.Euler(lookRotation.x, lookRotation.y, 0);
    }

    // DEBUG gizmos draw
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + MoveDirection.y, transform.position.z));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + MoveDirection.x, transform.position.y, transform.position.z + MoveDirection.z));
    }
}