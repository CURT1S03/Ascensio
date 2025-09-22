using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SlidingDoorController : MonoBehaviour
{
    private Animator animator;

    private const string PLAYER_NEARBY_PARAM = "IsPlayerNearby";

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            BallCollector target = other.attachedRigidbody.gameObject.GetComponent<BallCollector>();

            if (target != null)
            {
                animator.SetBool(PLAYER_NEARBY_PARAM, true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            BallCollector target = other.attachedRigidbody.gameObject.GetComponent<BallCollector>();

            if (target != null)
            {
                animator.SetBool(PLAYER_NEARBY_PARAM, false);
            }
        }
    }
}