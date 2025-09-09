using UnityEngine;

// This ensures the GameObject must have a SpriteRenderer component.
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float attackRange = 5f;
    public float playerPower = 25f; // How much damage the player deals
    public float health = 100f;   // Player's health

    // Invincibility Frame (I-Frame) settings
    public float invincibilityDuration = 1.5f;
    private bool isInvincible = false;
    private float invincibilityTimer;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component attached to this GameObject.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, vertical, 0);
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // Attacking on key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        // Handle Invincibility Frames
        if (isInvincible)
        {
            // Count down the timer
            invincibilityTimer -= Time.deltaTime;

            // Calculate the color fade from black to white
            float progress = 1.0f - (invincibilityTimer / invincibilityDuration);
            spriteRenderer.color = Color.Lerp(Color.black, Color.white, progress);

            // Check if the invincibility period has ended
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                spriteRenderer.color = Color.white; // Reset to fully white
            }
        }
    }

    void Attack()
    {
        EnemyAI nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Debug.Log("Player is attacking " + nearestEnemy.name);
            nearestEnemy.TakeDamage(playerPower);
        }
    }

    EnemyAI FindNearestEnemy()
    {
        EnemyAI[] enemies = FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        EnemyAI nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (EnemyAI enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, currentPos);
            if (distance < minDistance && distance <= attackRange)
            {
                nearest = enemy;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public void TakeDamage(float amount)
    {
        // If the player is invincible, ignore the damage.
        if (isInvincible)
        {
            return;
        }

        health -= amount;
        Debug.Log("Player took " + amount + " damage. Current health: " + health);

        // Start the invincibility period
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        spriteRenderer.color = Color.black; // Immediately show feedback

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died. Game Over!");
        // Here you would typically handle game over logic, like reloading the scene
        // or showing a game over screen. For now, we'll just disable the script.
        this.enabled = false;
    }
}