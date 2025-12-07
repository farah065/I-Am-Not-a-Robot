using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ShopManager : Singleton<ShopManager>
{
    public CardController[] CardControllers;
    public Trie Trie;
    public bool IsShopOpen => _shopUI.activeSelf;
    [SerializeField] private GameObject _shopUI;
    [SerializeField] private PowerupData[] _availablePowerups;
    [SerializeField] private Player2D _player;
    [SerializeField] private TMP_Text _skipButtonText;
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
            } while (_usedPowerupIndices.Contains(randomIndex) && !IsPowerupInInventory(_availablePowerups[randomIndex]));

            _usedPowerupIndices.Add(randomIndex);
            CardControllers[i].SetPowerupData(_availablePowerups[randomIndex]);

            Trie.Insert(_availablePowerups[randomIndex].DisplayName.ToLower().Replace(" ", ""));
        }

        ResetHighlight();
    }

    private bool IsPowerupInInventory(PowerupData powerupData)
    {
        foreach (var item in InventoryManager.Instance.InventorySlots)
        {
            if (!item.IsEmpty && item.PowerupData == powerupData)
                return true;
        }
        return false;
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
        if (_player.Coins >= card.PowerupData.Cost)
        {
            _player.AddCoins(-card.PowerupData.Cost);
            InventoryManager.Instance.AddItemToInventory(card.PowerupData);
            DisableShop();
        }
    }

    public void HighlightPrefix(int length)
    {
        string word = "Skip";
        length = Mathf.Clamp(length, 0, word.Length);

        string prefix = word.Substring(0, length);
        string suffix = word.Substring(length);

        // Change this color to whatever highlight you want
        string coloredPrefix = $"<color=#E0F8CF>{prefix}</color>";

        _skipButtonText.text = coloredPrefix + suffix;
    }

    public void ResetHighlight()
    {
        _skipButtonText.text = "Skip";
    }
}
