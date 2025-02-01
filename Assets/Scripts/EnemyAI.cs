using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f; // Movement speed of the enemy
    [SerializeField] private float agroRange = 5f; // Range within which the enemy will chase the player
    [SerializeField] private float attackRange = 1.5f; // Range within which the enemy will attack the player
    [SerializeField] private int damage = 10; // Damage dealt to the player on hit
    [SerializeField] private float attackCooldown = 2f; // Cooldown between attacks

    [SerializeField] private int maxHealth = 100; // Maximum health of the enemy
    private int currentHealth;

    private SpriteRenderer _mySpriteRenderer;
    private Transform player; // Reference to the player's transform
    private Rigidbody2D rb2d; // Reference to the Rigidbody2D component
    private Animator animator; // Reference to the Animator component
    private float timeSinceLastAttack; // Timer to track attack cooldown

    public GameObject bloodPrefab; // Assign in Inspector

    private void Start()
    {
        player = Player.Instance.transform; // Get the player's transform from the singleton
        rb2d = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        animator = GetComponent<Animator>(); // Get the Animator component
        timeSinceLastAttack = attackCooldown; // Initialize attack cooldown
        _mySpriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (player == null) return; // If the player is null, exit the method
        float distanceToPlayer = Vector2.Distance(transform.position,
            player.position); // Calculate distance to player
        if (distanceToPlayer <= agroRange) // If within chase range, chase the player
        {
            ChasePlayer(distanceToPlayer);
        }
        else // Otherwise, stop chasing
        {
            StopChasing();
        }

// Handle attack cooldown
        if (timeSinceLastAttack < attackCooldown)
        {
            timeSinceLastAttack += Time.deltaTime; // Increment the cooldown timer
        }
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        Vector2 direction = (player.position - transform.position).normalized; // Calculate direction to player
        rb2d.linearVelocity = direction * moveSpeed; // Move towards the player
        animator.SetBool("isMoving", true); // Set the isMoving parameter to true
        AdjustFacingDirection();
// Attack if within range and cooldown is over
        if (distanceToPlayer <= attackRange && timeSinceLastAttack >=
            attackCooldown)
        {
            AttackPlayer();
        }
    }

    private void StopChasing()
    {
        rb2d.linearVelocity = Vector2.zero; // Stop movement
        animator.SetBool("isMoving", false); // Set the isMoving parameter to false
    }

    private void AttackPlayer()
    {
        animator.SetTrigger("isAttack"); // Trigger the attack animation
        Player.Instance.TakeDamage(damage); // Deal damage to the player
        timeSinceLastAttack = 0f; // Reset the attack cooldown
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce the enemy's health
        Debug.Log($"Enemy took {damage} damage. Current health: {currentHealth}");

        if (bloodPrefab != null)
        {
            GameObject blood = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
            Destroy(blood, 0.5f);
        }

        if (currentHealth <= 0)
        {
            Die(); // Call the Die method if health is zero or below
        }
    }

    private void Die()
    {
        Debug.Log("Enemy is DEAD!"); // Log the enemy's death
        animator.SetTrigger("isDead"); // Trigger the death animation
        Destroy(gameObject,
            animator.GetCurrentAnimatorStateInfo(0).length); // Destroy the enemy after the death animation ends
    }

    private void AdjustFacingDirection()
    {
        // Orient character to the mouse position left or right to him
        _mySpriteRenderer.flipX = rb2d.linearVelocity.x < 0;
    }
}