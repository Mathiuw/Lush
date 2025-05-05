using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{
    Animator animator;
    [SerializeField] float distanceToGround;
    [SerializeField] LayerMask layerMask;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (!animator)
        {
            Debug.LogError("Cant find animator");
            enabled = false;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);

        RaycastHit hit;

        // Left foot
        Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, distanceToGround, layerMask))
        {
            Vector3 footPosition = hit.point;
            footPosition.y += distanceToGround;
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        }

        // Right foot
        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, distanceToGround, layerMask))
        {
            Vector3 footPosition = hit.point;
            footPosition.y += distanceToGround;
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        }
    }
}
