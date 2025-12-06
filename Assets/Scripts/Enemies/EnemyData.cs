using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float MaxHp = 15f;
    public float BaseSpeed = 1f;
    public float BaseCoinDropChance = 0.3f;
}
