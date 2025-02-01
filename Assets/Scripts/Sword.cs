using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
    // Singleton instance for the player
    public static Sword Instance { get; private set; }


    [SerializeField] private float hitCooldown = 0.5f; // Cooldown time between hits in seconds
    private Animator myAnimator;
    private PlayerSpaceControls playerControls;
    private Player player;
    private Weapon weapon;
    private PolygonCollider2D swordCollider; // Reference to the sword's collider
    private bool canHit = true; // Flag to control if the sword can hit

    [SerializeField] private int damage = 10;
    [SerializeField] private int manaRestoredOnHit = 5;
    
    // This method is called when the object is first created.
    private void Awake()
    {
        // Singleton pattern to ensure only one player instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        myAnimator = GetComponent<Animator>();
        playerControls = new PlayerSpaceControls();
        player = GetComponentInParent<Player>();
        weapon = GetComponentInParent<Weapon>();
        swordCollider = GetComponent<PolygonCollider2D>(); // Get the reference to the collider
        swordCollider.enabled = false; // Initially disable the collider
    }

    // This method is called when the GameObject becomes enabled and active.
    private void OnEnable()
    {
        playerControls.Enable(); // Enable the player controls
    }

    // Update is called once per frame.
    private void Update()
    {
    }

    // This method is called before the first frame update.
    void Start()
    {
        playerControls.Combat.Attack.started += _ => Attack(); // Subscribe to the attack input event
    }

    // This method is called when another collider enters the trigger collider attached to the same GameObject as this script.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canHit)
        {
            if (other.tag == "Enemy")
            {
                Debug.Log("HIT enemy");
                EnemyAI enemy = other.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Player.Instance.TakeBonuses(0, manaRestoredOnHit);
                }

                StartCoroutine(HitCooldown());
            } else if (other.tag == "DestroyableItem")
            {
                Debug.Log("HIT destroyable item");
                DestroyableItem item = other.GetComponent<DestroyableItem>();
                if (item != null)
                {
                    item.TakeDamage(damage);
                }
            }
            
        }
    }

// This method triggers the attack animation.
    private void Attack()
    {
        myAnimator.SetTrigger("isAttack"); // Trigger the attack animation
        EnableColliderDuringAttack(); // Enable the collider when attacking
    }

    // This method enables the sword's collider during the attack.
    private void EnableColliderDuringAttack()
    {
        swordCollider.enabled = true; // Enable the collider at the start of the attack
        // Use a coroutine to disable the collider after the attack animation ends
        StartCoroutine(DisableColliderAfterDelay());
    }

    // This coroutine disables the collider after the attack animation duration.
    private IEnumerator DisableColliderAfterDelay()
    {
        // Wait for the duration of the attack animation
        yield return new
            WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length);
// Ensure the collider is disabled after the attack animation ends
        swordCollider.enabled = false;
    }

    // This coroutine manages the cooldown period between hits.
    private IEnumerator HitCooldown()
    {
        canHit = false; // Disable hitting during cooldown
        // Wait for the cooldown period using a coroutine
        yield return new WaitForSeconds(hitCooldown);
        canHit = true; // Re-enable hitting after cooldown
    }
    
    private void OnDisable()
    {
        playerControls.Disable();
    }
}