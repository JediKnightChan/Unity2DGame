using UnityEngine;

public class DestroyableItem : MonoBehaviour
{
    [SerializeField] private int healingAmount;
    [SerializeField] private int manaAmount;

    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    [SerializeField] private GameObject vfxPrefab;
    
    [SerializeField] private bool bEndsGame = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (vfxPrefab != null)
        {
            GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 0.5f);
        }

        if (currentHealth < 0)
        {
            GiveBonusesToPlayer();
            Die();
        }
    }

    void GiveBonusesToPlayer()
    {
        Player.Instance.TakeBonuses(healingAmount, manaAmount);

        if (bEndsGame)
        {
            StartCoroutine(Player.Instance.ShowVictoryScreen());
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}