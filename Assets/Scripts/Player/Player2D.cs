using UnityEngine;

public class Player2D : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _centrePoint;

    public void FireBullet(Vector3 targetPosition, int damage, float _multiplier, Enemy target)
    {
        Vector3 direction = (targetPosition - _centrePoint.position).normalized;
        GameObject bulletGameObj = Instantiate(_bulletPrefab, _centrePoint.position, Quaternion.LookRotation(Vector3.forward, direction));
        Bullet bullet = bulletGameObj.GetComponent<Bullet>();
        bullet.Initialise(direction, damage, _multiplier, target);
    }
}
