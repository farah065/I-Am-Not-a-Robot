using UnityEngine;

public class HealthUIController : Singleton<HealthUIController>
{
    [SerializeField] private GameObject[] _hearts;

    public void UpdateHealth(int currentHealth)
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            _hearts[i].SetActive(i < currentHealth);
        }
    }
}
