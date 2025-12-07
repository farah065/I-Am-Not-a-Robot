using NUnit.Framework;
using System.Collections;
using UnityEngine;

// 2D Player controller with health, coins, transitions, and interaction logic.
// Uses Singleton pattern for global access.
public class Player2D : Singleton<Player2D>
{
    // Player currency
    public int Coins;

    // Player health (0–3)
    public int Hp;

    // True when the player is moving upward during a scene transition
    public bool IsTransitioning = false;

    // True when the player has walked into an exit trigger
    public bool HasReachedExit = false;

    // Set when freeze-type powerups are active (enables freezing bullets)
    public bool CanFreeze = false;

    // Prefab for bullets fired by the player
    [SerializeField] private GameObject _bulletPrefab;

    // Position from which bullets are fired
    [SerializeField] private Transform _centrePoint;

    // Animator controlling player animations
    [SerializeField] private Animator _animator;

    // Rigidbody for physics-based movement
    [SerializeField] private Rigidbody2D _rb;

    // Renderer for showing/hiding player sprite
    [SerializeField] private SpriteRenderer _spriteRenderer;

    // Particle effect played on death
    [SerializeField] private ParticleSystem _deathEffect;

    // Used to determine when the player is allowed to stop moving during transitions
    private bool _canStop = false;

    private void Start()
    {
        // Initialize player values at game start
        Initialise();
    }

    // Sets initial HP, coins, UI, and resets any powerup effects
    public void Initialise()
    {
        Hp = 3;
        Coins = 0;
        HealthUIController.Instance.UpdateHealth(Hp);
        CoinUIController.Instance.UpdateCoinCount(Coins);
        ResetPowerupEffects();
        _spriteRenderer.enabled = true;
    }

    private void Update()
    {
        // Handle upward movement during scene transition animation
        if (IsTransitioning)
        {
            _rb.linearVelocity = new Vector2(0, 2f);

            // Stop when reaching a certain Y-level *and* when allowed to stop
            if (transform.localPosition.y >= 0 && _canStop)
            {
                IsTransitioning = false;
                HasReachedExit = false;
                _canStop = false;
                _animator.SetBool("IsWalkingUp", false);
                _rb.linearVelocity = Vector2.zero;
            }
        }
    }

    // Fires a bullet toward a target enemy
    public void FireBullet(Vector3 targetPosition, int damage, float _multiplier, Enemy target)
    {
        // Get direction toward target
        Vector3 direction = (targetPosition - _centrePoint.position).normalized;

        // Spawn bullet facing that direction
        GameObject bulletGameObj = Instantiate(
            _bulletPrefab,
            _centrePoint.position,
            Quaternion.LookRotation(Vector3.forward, direction)
        );

        // Initialize bullet with damage, multiplier, target, and freeze ability
        Bullet bullet = bulletGameObj.GetComponent<Bullet>();
        bullet.Initialise(direction, damage, _multiplier, target, CanFreeze);
    }

    // Clears temporary powerup effects
    public void ResetPowerupEffects()
    {
        CanFreeze = false;
    }

    // Reduces player HP and triggers death if HP reaches 0
    public void TakeDamage()
    {
        if (Hp > 0)
        {
            Hp--;
            HealthUIController.Instance.UpdateHealth(Hp);

            // If HP reached zero, start death sequence
            if (Hp <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    // Increases HP but cannot exceed max value of 3
    public void Heal()
    {
        Hp = Mathf.Min(Hp + 1, 3);
        HealthUIController.Instance.UpdateHealth(Hp);
    }

    // Plays death animation/effects and ends the game
    private IEnumerator Die()
    {
        _spriteRenderer.enabled = false;
        _deathEffect.Play();
        yield return new WaitForSeconds(2f);
        BrowserController.Instance.HideGame();
    }

    // Adds coins and updates UI
    public void AddCoins(int amount)
    {
        Coins += amount;
        CoinUIController.Instance.UpdateCoinCount(Coins);
    }

    // Begins upward movement and transition animation
    public void PlaySceneTransitionAnimation()
    {
        IsTransitioning = true;
        _animator.SetBool("IsWalkingUp", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When hitting the exit trigger, reposition player and flag that exit was reached
        if (collision.CompareTag("Exit"))
        {
            transform.localPosition = new Vector3(0, -6.6f, 0);
            HasReachedExit = true;
        }
        // Stop movement during transition when hitting designated stop trigger
        else if (collision.CompareTag("Stop"))
        {
            _canStop = true;
        }
    }
}
