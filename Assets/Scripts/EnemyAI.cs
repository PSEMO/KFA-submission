using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Vector3[] waypoints; // Store positions, not transforms
    private int currentWaypointIndex = 0;
    private bool hasReachedDestination = false;
    private PlayerController player;
    SpriteRenderer spriteRenderer;

    // Stats
    float speed = 0.2f;
    float health = 100;
    float power = 10;

    // Settings
    public float pathOffsetRadius = 1f; // Max random offset for the path

    // Attacking Settings
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    private float nextAttackTime = 0f;

    void Awake()
    {
        // Generate a single, consistent random offset for this enemy's entire path
        float randomYOffset = Random.Range(-pathOffsetRadius, pathOffsetRadius);
        Vector3 pathOffset = new Vector3(0, randomYOffset, 0);

        transform.position += pathOffset;

        GameObject roadsParent = GameObject.Find("roads");

        if (roadsParent != null)
        {
            int waypointCount = roadsParent.transform.childCount;
            waypoints = new Vector3[waypointCount];

            // CHILD'S POSITION + the UNIQUE OFFSET
            for (int i = 0; i < waypointCount; i++)
            {
                waypoints[i] = roadsParent.transform.GetChild(i).position + pathOffset;
            }

            if (waypointCount == 0)
            {
                Debug.LogWarning("The 'roads' object has no children to use as waypoints.", this);
            }
        }
        else
        {
            Debug.LogError("Could not find a GameObject named 'roads' in the scene. Enemy AI has no waypoints.", this);
            waypoints = new Vector3[0];
        }
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //make color's red component random therefor make stats random
        spriteRenderer.color = new Color(Random.value, spriteRenderer.color.g, spriteRenderer.color.b);
        setStatsByColor();
    }

    void Update()
    {
        // haven't reached destination, move
        if (!hasReachedDestination)
        {
            if (waypoints.Length > 0)
            {
                MoveTowardsWaypoint();

                if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex]) < 0.1f)
                {
                    GoToNextWaypoint();
                }
            }
        }
        // reached destination, start attack
        else
        {
            if (Time.time >= nextAttackTime)
            {
                AttackPlayer();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void setStatsByColor()
    {
        float statMultiplier = (spriteRenderer.color.r + 0.1f) / 0.6f;
        speed *= statMultiplier;
        health *= statMultiplier;
        power *= statMultiplier;
    }

    void MoveTowardsWaypoint()
    {
        Vector3 targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void GoToNextWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length - 1)
        {
            hasReachedDestination = true;
            Debug.Log(gameObject.name + " has reached the destination and is now hostile.");
        }
        else
        {
            currentWaypointIndex++;
        }
    }

    void AttackPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRange)
        {
            Debug.Log(gameObject.name + " attacks the player!");
            player.TakeDamage(power);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log(gameObject.name + " took " + amount + " damage. Current health: " + health);
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has been destroyed.");
        // You could instantiate an explosion or play a sound here before destroying
        Destroy(gameObject);
    }
}