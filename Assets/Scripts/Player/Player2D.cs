using UnityEngine;

public class Player2D : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _centrePoint;
    private int _hp;

    private void Start()
    {
        _hp = 3;
    }

    public void FireBullet(Vector3 targetPosition, int damage, float _multiplier, Enemy target)
    {
        Vector3 direction = (targetPosition - _centrePoint.position).normalized;
        GameObject bulletGameObj = Instantiate(_bulletPrefab, _centrePoint.position, Quaternion.LookRotation(Vector3.forward, direction));
        Bullet bullet = bulletGameObj.GetComponent<Bullet>();
        bullet.Initialise(direction, damage, _multiplier, target);
    }

    public void TakeDamage()
    {
        _hp--;
        HealthUIController.Instance.UpdateHealth(_hp);
    }
}
