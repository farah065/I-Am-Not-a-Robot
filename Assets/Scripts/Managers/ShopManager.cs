using UnityEngine;
using System.Collections.Generic;

public class ShopManager : Singleton<ShopManager>
{
    public CardController[] CardControllers;
    public Trie Trie;
    public bool IsShopOpen => _shopUI.activeSelf;
    [SerializeField] private GameObject _shopUI;
    [SerializeField] private PowerupData[] _availablePowerups;
    private List<int> _usedPowerupIndices;

    public void EnableShop()
    {
        Trie = new Trie();
        _shopUI.SetActive(true);
        _usedPowerupIndices = new List<int>();

        for (int i = 0; i < CardControllers.Length; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, _availablePowerups.Length);
            } while (_usedPowerupIndices.Contains(randomIndex));

            _usedPowerupIndices.Add(randomIndex);
            CardControllers[i].SetPowerupData(_availablePowerups[randomIndex]);

            Trie.Insert(_availablePowerups[randomIndex].DisplayName.ToLower().Replace(" ", ""));
        }
    }

    public void DisableShop()
    {
        _shopUI.SetActive(false);
        StartCoroutine(GameManager.Instance.OnShopClosed());
    }

    public CardController FindCardByNormalizedName(string normalized)
    {
        foreach (var card in CardControllers)
            if (card.NormalisedName == normalized)
                return card;

        return null;
    }

    public void Purchase(CardController card)
    {
        InventoryManager.Instance.AddItemToInventory(card.PowerupData);
        DisableShop();
    }
}
