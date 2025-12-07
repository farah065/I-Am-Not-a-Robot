using NUnit.Framework;
using UnityEngine;

public class Player2D : Singleton<Player2D>
{
    public int Coins;
    public int Hp;
    public bool IsTransitioning = false;
    public bool HasReachedExit = false;
    public bool CanFreeze = false;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _centrePoint;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private ParticleSystem _deathEffect;
    private bool _canStop = false;

    private void Start()
    {
        Hp = 3;
        Coins = 0;
    }

    private void Update()
    {
        if (IsTransitioning)
        {
            _rb.linearVelocity = new Vector2(0, 2f);
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

    public void FireBullet(Vector3 targetPosition, int damage, float _multiplier, Enemy target)
    {
        Vector3 direction = (targetPosition - _centrePoint.position).normalized;
        GameObject bulletGameObj = Instantiate(_bulletPrefab, _centrePoint.position, Quaternion.LookRotation(Vector3.forward, direction));
        Bullet bullet = bulletGameObj.GetComponent<Bullet>();
        bullet.Initialise(direction, damage, _multiplier, target, CanFreeze);
    }

    public void ResetPowerupEffects()
    {
        CanFreeze = false;
    }

    public void TakeDamage()
    {
        if (Hp > 0)
        {
            Hp--;
            HealthUIController.Instance.UpdateHealth(Hp);
            if (Hp <= 0)
            {
                Die();
            }
        }
    }

    public void Heal()
    {
        Hp = Mathf.Min(Hp + 1, 3);
        HealthUIController.Instance.UpdateHealth(Hp);
    }

    private void Die()
    {
        _spriteRenderer.enabled = false;
        _deathEffect.Play();
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        CoinUIController.Instance.UpdateCoinCount(Coins);
    }

    public void PlaySceneTransitionAnimation()
    {
        IsTransitioning = true;
        _animator.SetBool("IsWalkingUp", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Exit"))
        {
            transform.localPosition = new Vector3(0, -6.6f, 0);
            HasReachedExit = true;
        }
        else if (collision.CompareTag("Stop"))
        {
            _canStop = true;
        }
    }
}
