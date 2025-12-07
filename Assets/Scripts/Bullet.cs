using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed = 10f;
    private Enemy _targetEnemy;
    private int _damage;
    private float _multiplier;
    private float _lifetime = 5f;
    private bool _shouldFreeze;

    public void Initialise(Vector3 direction, int damage, float multiplier, Enemy target, bool shouldFreeze)
    {
        _damage = damage;
        _multiplier = multiplier;
        _targetEnemy = target;
        _rb.linearVelocity = direction * _speed;
        _shouldFreeze = shouldFreeze;
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && enemy == _targetEnemy)
            {
                enemy.TakeDamage(_damage, _multiplier);
                if (_shouldFreeze)
                {
                    enemy.FrozenTimer = 1f;
                }
                Destroy(gameObject);
            }
        }
    }
}
