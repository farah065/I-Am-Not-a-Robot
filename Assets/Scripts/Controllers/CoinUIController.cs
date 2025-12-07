using TMPro;
using UnityEngine;

public class CoinUIController : Singleton<CoinUIController>
{
    [SerializeField] private TMP_Text _coinCountText;

    public void UpdateCoinCount(int newCoinCount)
    {
        _coinCountText.text = newCoinCount.ToString();
    }
}
