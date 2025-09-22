using UnityEngine;
using UnityEngine.AI; 

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class MinionAI : MonoBehaviour
{
    private enum AIState
    {
        PatrolStaticWaypoints,
        InterceptMovingWaypoint
    }
    private AIState currentState;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private VelocityReporter movingWaypointVelocityReporter;

    [Header("Static Waypoint Patrol (State 0)")]
    public GameObject[] staticWaypoints;
    private int currentStaticWaypointIndex = -1;

    [Header("Moving Waypoint Intercept (State 1)")]
    public GameObject movingWaypoint;
    public GameObject destinationTracker;
    public float captureDistance = 1.5f;
    public float maxPredictionTime = 2.0f;


    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (movingWaypoint != null)
        {
            movingWaypointVelocityReporter = movingWaypoint.GetComponent<VelocityReporter>();
        }

        destinationTracker.SetActive(false);
        currentStaticWaypointIndex = -1;
        currentState = AIState.PatrolStaticWaypoints;
        SetNextStaticWaypoint();
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.PatrolStaticWaypoints:
                Update_PatrolState();
                break;
            case AIState.InterceptMovingWaypoint:
                Update_InterceptState();
                break;
        }

        animator.SetFloat("vely", navMeshAgent.velocity.magnitude / navMeshAgent.speed);
    }

    private void Update_PatrolState()
    {
        destinationTracker.transform.position = navMeshAgent.destination;
        
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            if (currentStaticWaypointIndex == staticWaypoints.Length - 1)
            {
                currentState = AIState.InterceptMovingWaypoint;
                destinationTracker.SetActive(true);
            }
            else
            {
                SetNextStaticWaypoint();
            }
        }
    }

    private void Update_InterceptState()
    {
        float distanceToTarget = Vector3.Distance(transform.position, movingWaypoint.transform.position);
        if (distanceToTarget <= captureDistance)
        {
            currentState = AIState.PatrolStaticWaypoints;
            currentStaticWaypointIndex = -1;
            SetNextStaticWaypoint();
            destinationTracker.SetActive(false);
            return;
        }

        float lookaheadTime = Mathf.Clamp(distanceToTarget / navMeshAgent.speed, 0, maxPredictionTime);

        Vector3 predictedPosition = movingWaypoint.transform.position + (movingWaypointVelocityReporter.velocity * lookaheadTime);

        NavMeshHit hit;
        if (NavMesh.Raycast(movingWaypoint.transform.position, predictedPosition, out hit, NavMesh.AllAreas))
        {
            predictedPosition = hit.position;
        }

        destinationTracker.transform.position = predictedPosition;
        navMeshAgent.SetDestination(predictedPosition);
    }

    private void SetNextStaticWaypoint()
    {
        currentStaticWaypointIndex = (currentStaticWaypointIndex + 1) % staticWaypoints.Length;
        navMeshAgent.SetDestination(staticWaypoints[currentStaticWaypointIndex].transform.position);
    }
}
