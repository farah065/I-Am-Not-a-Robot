using UnityEngine;
using System.Collections.Generic;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private GameObject _shopUI;
    [SerializeField] private PowerupData[] _availablePowerups;
    [SerializeField] private CardController[] _cardControllers;
    private List<int> _usedPowerupIndices;

    private void EnableShop()
    {
        _shopUI.SetActive(true);
        _usedPowerupIndices = new List<int>();

        for (int i = 0; i < _cardControllers.Length; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, _availablePowerups.Length);
            } while (_usedPowerupIndices.Contains(randomIndex));

            _usedPowerupIndices.Add(randomIndex);
            _cardControllers[i].SetPowerupData(_availablePowerups[randomIndex]);
        }
    }

    private void DisableShop()
    {
        _shopUI.SetActive(false);
    }
}
