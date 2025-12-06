using UnityEngine;

public enum PowerupType
{
    Pierce,
    Lightining,
    Poison,
    Freeze,
    Bandage,
    SlowMo,
    CoinMagnet,
    CommonMult,
    UncommonMult,
    Autocorrect,
    Revival,
    Foresight
}

[CreateAssetMenu(fileName = "PowerupData", menuName = "ScriptableObjects/Powerup")]
public class PowerupData : ScriptableObject
{
    public string TypableName;
    public PowerupType Type;
    public Sprite Icon;
    public string Description;
    public int Cost;
}
