using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    private bool isLaunched = false;
    private Vector2 myDirection;

    private Rigidbody2D rb2d; // Reference to the Rigidbody2D component
    private BoxCollider2D col;
    private SpriteRenderer _mySpriteRenderer;
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        col = GetComponent<BoxCollider2D>();
        _mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (isLaunched)
        {
            rb2d.linearVelocity = myDirection * speed;
            _mySpriteRenderer.flipX = myDirection.x < 0;
        }
    }

    public void Launch(Vector2 Direction)
    {
        isLaunched = true;
        myDirection = Direction;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"trigger enter {other}");
        if (isLaunched)
        {
            if (other.tag == "Enemy")
            {
                Debug.Log("HIT enemy");
                EnemyAI enemy = other.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            else if (other.tag == "DestroyableItem")
            {
                Debug.Log("HIT destroyable item");
                DestroyableItem item = other.GetComponent<DestroyableItem>();
                if (item != null)
                {
                    item.TakeDamage(damage);
                }
            }

            Destroy(this);
        }
    }
}