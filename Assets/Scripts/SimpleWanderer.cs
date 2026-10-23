using UnityEngine;

// Simple wandering: walks forward, occasionally turns randomly. No NavMesh needed.
// Pair with a "Walk In Place" animation that loops on the character.
public class SimpleWanderer : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 1.0f;

    [Header("Turning")]
    public float minTurnInterval = 3f;
    public float maxTurnInterval = 8f;
    public float maxTurnAngle = 90f;

    [Header("Bounds (don't wander too far)")]
    public Vector3 areaCenter = Vector3.zero;
    public float areaRadius = 15f;

    private float nextTurnTime;

    void Start()
    {
        ScheduleNextTurn();
    }

    void Update()
    {
        // Walk forward
        transform.position += transform.forward * walkSpeed * Time.deltaTime;

        // Time to turn?
        if (Time.time >= nextTurnTime)
        {
            float turnAngle = Random.Range(-maxTurnAngle, maxTurnAngle);
            transform.Rotate(0f, turnAngle, 0f);
            ScheduleNextTurn();
        }

        // If outside the area, turn back toward center
        Vector3 horizontalPos = new Vector3(transform.position.x, areaCenter.y, transform.position.z);
        if (Vector3.Distance(horizontalPos, areaCenter) > areaRadius)
        {
            Vector3 directionToCenter = (areaCenter - transform.position).normalized;
            directionToCenter.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToCenter);
            ScheduleNextTurn();
        }
    }

    void ScheduleNextTurn()
    {
        nextTurnTime = Time.time + Random.Range(minTurnInterval, maxTurnInterval);
    }
}