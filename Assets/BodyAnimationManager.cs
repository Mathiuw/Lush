using UnityEngine;

public class BodyAnimationManager : MonoBehaviour
{
    [SerializeField] Player player;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (!player) 
        {
            Debug.LogError("BodyAnimationManager cant find player");
            enabled = false;
        }
    }

    private void Update()
    {
        animator.SetFloat("x", player.GetMoveDirection().x);
        animator.SetFloat("y", player.GetMoveDirection().z);
    }
}
