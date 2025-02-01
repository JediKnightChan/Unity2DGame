using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Singleton instance for the player
    public static Player Instance { get; private set; }

    [SerializeField] private float speed = 5.0f;
    [SerializeField] private int maxHealth = 100; // Maximum health of the player
    private int currentHealth; // Current health of the player
    [SerializeField] private int maxMana = 100; // Maximum mana of the player
    private int currentMana; // Current mana of the player

    public GameObject manaProjectilePrefab;
    [SerializeField] private int manaProjectileCost = 10;

    private PlayerSpaceControls _playerControls;
    private Vector2 _movement;
    private Rigidbody2D _rigidbody2D;
    private Animator _myAnimator;
    private SpriteRenderer _mySpriteRenderer;

    private static readonly int MoveY = Animator.StringToHash("moveY");
    private static readonly int MoveX = Animator.StringToHash("moveX");

    public TextMeshProUGUI healthDisplay; // Display health
    public TextMeshProUGUI manaDisplay; // Display health
    public GameObject bloodPrefab; // Assign in Inspector
    public GameObject healingPrefab;
    public GameObject deathScreenParent;

    public Sprite victorySprite;

    public bool orientationLeft;

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

        _playerControls = new PlayerSpaceControls();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _myAnimator = GetComponent<Animator>();
        _mySpriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth; // Initialize health
        currentMana = maxMana;

        _playerControls.Combat.RangeAttack.started += _ => ManaAttack();
    }

    private void Start()
    {
        UpdateUI();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        healthDisplay = GameObject.Find("healthDisplay").GetComponent<TextMeshProUGUI>();
        manaDisplay = GameObject.Find("manaDisplay").GetComponent<TextMeshProUGUI>();
        deathScreenParent = GameObject.Find("DiedScreen");
        deathScreenParent.SetActive(false);
        Debug.Log($"Updating UI, health {healthDisplay}, death screen {deathScreenParent}");
    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessPlayerInput();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        ApplyMovement();

        healthDisplay.text = string.Format("Health: {0}", currentHealth);
        manaDisplay.text = string.Format("Mana: {0}", currentMana);
    }

    private void ProcessPlayerInput()
    {
        _movement = _playerControls.Movement.Move.ReadValue<Vector2>();

        // Update animation params
        _myAnimator.SetFloat(MoveX, _movement.x);
        _myAnimator.SetFloat(MoveY, _movement.y);
    }


    private void ApplyMovement()
    {
        _rigidbody2D.MovePosition(_rigidbody2D.position + _movement * (speed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        // Orient character left or right to him
        orientationLeft = _movement.x < 0;
        _mySpriteRenderer.flipX = orientationLeft;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce the player's health
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

        if (bloodPrefab != null)
        {
            GameObject blood = Instantiate(bloodPrefab, transform.position, Quaternion.identity);
            Destroy(blood, 0.5f);
        }

        if (currentHealth <= 0) // If health is zero or below
        {
            Die(); // Call the Die method
        }
    }

    public void TakeBonuses(int healing, int mana)
    {
        currentHealth = Mathf.Clamp(currentHealth + healing, 0, maxHealth);
        currentMana = Mathf.Clamp(currentMana + mana, 0, maxMana);

        if (healingPrefab != null)
        {
            GameObject vfx = Instantiate(healingPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 0.5f);
        }
    }

    private void Die()
    {
        StartCoroutine(ShowDeathScreen());
    }

    IEnumerator ShowDeathScreen()
    {
        Debug.Log($"Death screen, {deathScreenParent}");
        deathScreenParent.SetActive(true); // Show the entire UI group
        yield return new WaitForSeconds(2f);

        Instance = null; // Remove singleton reference
        Destroy(gameObject);

        SceneManager.LoadScene("GUI");
    }

    public IEnumerator ShowVictoryScreen()
    {
        deathScreenParent.SetActive(true); // Show the entire UI group
        Image image = deathScreenParent.transform.GetChild(1).GetComponent<Image>();
        image.sprite = victorySprite;

        yield return new WaitForSeconds(2f);

        Instance = null; // Remove singleton reference
        Destroy(gameObject);

        SceneManager.LoadScene("GUI");
    }

    private void OnDisable()
    {
        _playerControls.Disable();
    }

    private void ManaAttack()
    {
        Debug.Log("Mana attack wanted");
        if (currentMana >= manaProjectileCost)
        {
            Vector3 offset = Vector3.right * 2;
            Vector2 Direction = Vector2.right;
            if (orientationLeft)
            {
                offset *= -1;
                Direction *= -1;
            }

            GameObject bullet = Instantiate(manaProjectilePrefab, transform.position + offset, Quaternion.identity);
            Bullet BulletComponent = bullet.GetComponent<Bullet>();
            BulletComponent.Launch(Direction);

            currentMana -= manaProjectileCost;
        }
    }
}