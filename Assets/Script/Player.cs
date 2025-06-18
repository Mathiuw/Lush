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
    Vector3 velocity = Vector3.zero;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float sphereRadius = 0.2f;
    [SerializeField] float sphereOffset = 0f;
    bool grounded = false;

    // Camera movement variables
    [Header("Camera Movement")]
    [SerializeField] float sensibility = 20f;
    [SerializeField] Transform headPivot;
    Vector2 lookInput = Vector2.zero;
    float xlookRotation = 0f;
    float yBodyRotation = 0f;

    // Head bob variables
    [Header("Head Bob")]
    [SerializeField] bool headBob = true;
    [SerializeField] float amplitude = 0.1f; 
    [SerializeField] float frequency = 2f;
    [SerializeField] bool bobX = false, bobY = true;
    float bobTime = 0f;

    // Sound Variables
    [Header("Audio")]
    [SerializeField] float footstepFadeSpeed = 10;
    AudioSource footstepSoundSource;
    float maxVolume;

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

    public InputActions_Player GetInput() 
    {
        return inputActions;
    }

    public float GetSensibility() 
    {
        return sensibility;
    }

    public void SetSensibiity(float sensibility) 
    {
        this.sensibility = sensibility;
    }

    public Vector3 GetMoveDirection() 
    {
        return MoveDirection;
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

        inputActions.Disable();
    }

    private void Awake()
    {
        // Lock Cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Get character controller component
        characterController = GetComponent<CharacterController>();

        // Get the audio source component from player feet
        footstepSoundSource = GetComponentInChildren<AudioSource>();
        maxVolume = footstepSoundSource.volume;
        footstepSoundSource.volume = 0;

        if (!characterController)
        {
            Debug.LogError("Cant find character controller");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        // If input is disabled, dont update the camera position
        if (inputActions.asset.enabled)
        {
            // Apply camera movement
            CameraMovement();
        }

        velocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);

        // Head bob
        if (headBob && grounded)
        {
            bobTime += velocity.magnitude * Time.deltaTime;

            headPivot.localPosition = HeadBob(bobTime);
        }

        // Apply player movement
        PlayerMovement();

        //SetFootstepSound();
        FootstepAudioVolume();
    }

    private void PlayerMovement() 
    {
        // Gravity
        grounded = Physics.CheckSphere(transform.position + Vector3.down + Vector3.up * sphereOffset, sphereRadius, groundMask);

        if (!grounded)
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

        characterController.Move(speed * Time.deltaTime * MoveDirection.normalized);
    }

    private void CameraMovement()
    {
        float mouseX = lookInput.x * sensibility;
        float mouseY = lookInput.y * sensibility;

        xlookRotation -= mouseY * Time.deltaTime;
        yBodyRotation += mouseX * Time.deltaTime;

        // Clamp x rotation value
        xlookRotation = Math.Clamp(xlookRotation, -89, 89);

        transform.localRotation = Quaternion.Euler(0, yBodyRotation, 0);
        headPivot.localRotation = Quaternion.Euler(xlookRotation, 0, 0);
    }

    public void FocusPlayerCamera(Transform target) 
    {
        // Disable player input
        inputActions.Disable();

        // Rotate body to target
        Quaternion bodyRotation = transform.rotation;
        transform.LookAt(target);
        transform.rotation = Quaternion.Euler(new Vector3(bodyRotation.eulerAngles.x, transform.rotation.eulerAngles.y, bodyRotation.eulerAngles.z));

        // Rotate the camera to the target
        headPivot.LookAt(target);
        Debug.Log("Player focusing on target");
    }

    private Vector3 HeadBob(float time)
    {
        Vector3 position = Vector3.zero;

        if (bobX)
        {
            position.x += Mathf.Cos(time * frequency / 2) * amplitude;
        }
        if (bobY) 
        {
            position.y += Mathf.Sin(time * frequency) * amplitude;
        }

        return position;
    }

    private void FootstepAudioVolume() 
    {
        float desiredVolume;

        if (velocity.magnitude > 0 && grounded)
        {
            desiredVolume = maxVolume;
        }
        else
        {
            desiredVolume = 0;
        }

        footstepSoundSource.volume = Mathf.Lerp(footstepSoundSource.volume, desiredVolume, Time.deltaTime * footstepFadeSpeed);
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