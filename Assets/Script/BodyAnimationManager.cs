using UnityEngine;

public class BodyAnimationManager : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (!animator)
        {
            Debug.LogError("Animator not found");
            enabled = false;
        }
    }

    private void Start()
    {
        if (!characterController)
        {
            Debug.LogError("Character controller not found");
            enabled = false;
        }
    }

    private void Update()
    {
        // Set the blend tree state based on players character controller velocity
        animator.SetFloat("x", Vector3.Dot(characterController.transform.right, characterController.velocity.normalized));
        animator.SetFloat("y", Vector3.Dot(characterController.transform.forward, characterController.velocity.normalized));
    }
}
