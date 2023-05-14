using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmAgent : MonoBehaviour
{
    [Tooltip("The maximum speed at which an agent can move. This essentially limits how fast the agent can go.")]
    public float maxSpeed = 5f;
//    [Tooltip("The maximum force that can be applied to the agent. This essentially limits how fast the agent can change direction.")]
//    public float maxForce = 5f;
    [Tooltip("The radius within which other agents are considered 'neighbors'. Agents will only try to align with, separate from, and cohere with neighbors within this radius.")]
    public float neighborRadius = 2f;
    [Tooltip(". This affects how strongly agents try to move away from each other. A higher value will cause agents to try to create more distance between each other.")]
    public float separationWeight = 1.5f;
    [Tooltip("This affects how strongly agents try to align their velocities with each other. A higher value will cause agents to try to move more in the same direction as their neighbors.")]
    public float alignmentWeight = 1.0f;
    [Tooltip("Weight for the cohesion force. This affects how strongly agents try to move towards the average position of their neighbors. A higher value will cause agents to try to stay closer to the center of their group of neighbors.")]
    public float cohesionWeight = 1.0f;
    [Tooltip("Weight for the goal-seeking force. This affects how strongly agents try to move towards the goal (the swarmCenter). A higher value will cause agents to try to reach the goal more quickly")]
    public float goalSeekingWeight = 1.0f; // New weight for goal-seeking behavior
    [Tooltip("This is the goal that the agents are trying to reach. The goal-seeking behavior steers the agents towards this position.")]
    public Transform swarmCenter;

    private Vector3 velocity;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        swarmCenter = transform.parent;
        velocity = Random.insideUnitSphere * maxSpeed;
    }

    void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, neighborRadius);
        List<Rigidbody> context = new List<Rigidbody>();
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != GetComponent<Collider>())
            {
                Rigidbody hitRigidbody = hitCollider.attachedRigidbody;
                // Only add to context if the Rigidbody component exists
                if (hitRigidbody != null)
                {
                    context.Add(hitRigidbody);
                }
            }
        }

        Vector3 separation = CalculateSeparation(context) * separationWeight;
        Vector3 alignment = CalculateAlignment(context) * alignmentWeight;
        Vector3 cohesion = CalculateCohesion(context) * cohesionWeight;

        // Calculate goal-seeking force
        Vector3 goalDirection = (swarmCenter.position - transform.position).normalized;
        Vector3 goalForce = goalDirection * goalSeekingWeight;

        Vector3 acceleration = separation + alignment + cohesion + goalForce; // Added goalForce to the total force
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        rb.velocity = velocity;

        if (rb.velocity != Vector3.zero) {
            Vector3 direction = rb.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            lookRotation *= Quaternion.Euler(90, 0, 0); // This rotates the "front" from y-axis to z-axis
            transform.rotation = lookRotation;
        }

    }

    Vector3 CalculateSeparation(List<Rigidbody> context)
    {
        Vector3 separation = Vector3.zero;
        foreach (Rigidbody item in context)
        {
            Vector3 diff = transform.position - item.position;
            separation += diff / diff.sqrMagnitude;
        }
        return separation;
    }

    Vector3 CalculateAlignment(List<Rigidbody> context)
    {
        Vector3 alignment = Vector3.zero;
        foreach (Rigidbody item in context)
        {
            alignment += item.velocity;
        }
        if (context.Count > 0)
            alignment /= context.Count;
        return alignment;
    }

    Vector3 CalculateCohesion(List<Rigidbody> context)
    {
        Vector3 cohesion = Vector3.zero;
        foreach (Rigidbody item in context)
        {
            cohesion += item.position;
        }
        if (context.Count > 0)
            cohesion /= context.Count;
        cohesion -= transform.position;
        return cohesion;
    }
}
