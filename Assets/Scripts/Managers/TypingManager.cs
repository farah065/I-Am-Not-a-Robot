using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;

public class TypingManager : Singleton<TypingManager>
{
    public string Typed = "";
    [SerializeField] private Player2D _player;
    private Enemy _currentTarget = null;
    private Coin _currentCoinTarget = null;
    private PowerupData _currentInventoryTarget = null;
    private float _accuracyMultiplier = 1f;

    private Trie _enemyTrie => Spawner.Instance.EnemyTrie;
    private Trie _shopTrie => ShopManager.Instance.Trie;
    private Trie _coinTrie => Spawner.Instance.CoinTrie;
    private Trie _inventoryTrie => InventoryManager.Instance.InventoryTrie;


    private void Update()
    {
        foreach (var key in Keyboard.current.allKeys)
        {
            if (key == null) continue;
            if (key.wasPressedThisFrame)
            {
                char c = ConvertKey(key);
                if (c != '\0') HandleInput(c);
            }
        }
    }

    private char ConvertKey(KeyControl key)
    {
        if (key.name.Length == 1)
        {
            char c = key.name[0];
            if (c >= 'a' && c <= 'z')
                return c;
        }
        return '\0';
    }

    private void HandleInput(char c)
    {
        Typed += c;
        UpdateHighlights();

        if (!ShopManager.Instance.IsShopOpen)
        {
            HandleInputForEnemies();
            HandleInputForCoins();
            HandleInputForInventory();
        }
        else
        {
            HandleInputForShop();
            HandleInputForShopSkip();
        }
    }

    private void HandleInputForEnemies()
    {
        // If no target yet
        if (_currentTarget == null)
        {
            List<string> enemyMatches = _enemyTrie.Autocomplete(Typed);
            List<string> coinMatches = _coinTrie.Autocomplete(Typed);
            List<string> inventoryMatches = InventoryManager.Instance.InventoryTrie.Autocomplete(Typed);

            // If exactly one enemy matches → lock target
            if (enemyMatches.Count == 1)
            {
                _currentTarget = Spawner.Instance.FindEnemyByWord(enemyMatches[0]);
                Debug.Log("TARGET LOCKED: " + enemyMatches[0]);
                return;
            }

            // If no matches at all → typo (handled here only if zero coins too)
            if (enemyMatches.Count == 0 && coinMatches.Count == 0 && inventoryMatches.Count == 0)
            {
                Debug.Log("Wrong letter — matches no enemy or coin.");
                _accuracyMultiplier = 1f;

                Typed = Typed.Substring(0, Typed.Length - 1);
                UpdateHighlights();
            }

            return;
        }

        // If enemy IS targeted
        string targetWord = _currentTarget.CurrentWord;
        int correctPrefix = CountCorrectPrefix(Typed, targetWord);

        if (correctPrefix == Typed.Length)
        {
            if (Typed.Length == targetWord.Length)
            {
                Debug.Log("WORD COMPLETE: " + targetWord);

                _player.FireBullet(
                    _currentTarget.transform.position,
                    targetWord.Length,
                    _accuracyMultiplier,
                    _currentTarget
                );

                Typed = "";
                _currentTarget = null;
            }
        }
        else
        {
            Debug.Log("Wrong enemy letter!");
            _accuracyMultiplier = 1f;

            Typed = Typed.Substring(0, Typed.Length - 1);
            UpdateHighlights();
        }
    }


    private void HandleInputForCoins()
    {
        // If typing an enemy target, coins should not consume input
        if (_currentTarget != null)
            return;

        List<string> coinMatches = _coinTrie.Autocomplete(Typed);
        List<string> enemyMatches = _enemyTrie.Autocomplete(Typed);
        List<string> inventoryMatches = InventoryManager.Instance.InventoryTrie.Autocomplete(Typed);

        // If no matches anywhere, this is a true typo
        if (coinMatches.Count == 0 && enemyMatches.Count == 0 && inventoryMatches.Count == 0)
        {
            Debug.Log("Wrong letter — no enemy/coin match.");
            _accuracyMultiplier = 1f;

            Typed = Typed.Substring(0, Typed.Length - 1);
            UpdateHighlights();
            return;
        }

        // If exactly one coin matches AND no enemies match, we treat coin as target
        if (coinMatches.Count == 1 && enemyMatches.Count == 0)
        {
            Coin coin = Spawner.Instance.FindCoinByWord(coinMatches[0]);
            if (coin == null) return;

            int prefix = CountCorrectPrefix(Typed, coin.Word);

            // Correct prefix
            if (prefix == Typed.Length)
            {
                // Full match → collect coin!
                if (Typed.Length == coin.Word.Length)
                {
                    Debug.Log("COIN COLLECTED: " + coin.Word);

                    Spawner.Instance.CollectCoin(coin);
                    _player.AddCoins(1);

                    Typed = "";
                    _currentCoinTarget = null;
                    UpdateHighlights();
                }
                else
                {
                    _currentCoinTarget = coin;
                }
            }
            else
            {
                // Mistyped letter
                Debug.Log("Wrong coin letter!");
                _accuracyMultiplier = 1f;
                Typed = Typed.Substring(0, Typed.Length - 1);
                UpdateHighlights();
            }
        }
    }

    private void HandleInputForInventory()
    {
        // Do not use inventory items inside shop
        if (ShopManager.Instance.IsShopOpen)
            return;

        // If an enemy or coin is being typed, powerups should not consume input
        if (_currentTarget != null || _currentCoinTarget != null)
            return;

        List<string> inventoryMatches = InventoryManager.Instance.InventoryTrie.Autocomplete(Typed);
        List<string> enemyMatches = _enemyTrie.Autocomplete(Typed);
        List<string> coinMatches = _coinTrie.Autocomplete(Typed);

        // If no inventory matches AND no enemy matches AND no coin matches → true typo
        if (inventoryMatches.Count == 0 && enemyMatches.Count == 0 && coinMatches.Count == 0)
        {
            Debug.Log("Wrong letter — no enemy/coin/inventory match.");
            _accuracyMultiplier = 1f;

            Typed = Typed.Substring(0, Typed.Length - 1);
            UpdateHighlights();
            return;
        }

        // If multiple inventory matches → keep waiting
        if (inventoryMatches.Count > 1)
            return;

        // If exactly 1 match in inventory
        if (inventoryMatches.Count == 1)
        {
            string powerupName = inventoryMatches[0];

            // Check direct instance
            PowerupData powerup = InventoryManager.Instance.FindInventoryPowerup(powerupName);
            if (powerup == null)
                return;

            int prefix = CountCorrectPrefix(Typed, powerup.TypableName);

            // Good prefix
            if (prefix == Typed.Length)
            {
                // FULL WORD: USE POWERUP
                if (Typed.Length == powerup.TypableName.Length)
                {
                    Debug.Log("USED POWERUP: " + powerup.TypableName);

                    // TODO: Apply powerup effect here
                    InventoryManager.Instance.ApplyInventoryPowerup(powerup);

                    // consume input
                    Typed = "";
                    _currentInventoryTarget = null;
                    UpdateHighlights();
                }
                else
                {
                    _currentInventoryTarget = powerup;
                }
            }
            else
            {
                // Mistyped letter inside a powerup word
                Debug.Log("Wrong inventory letter!");
                _accuracyMultiplier = 1f;
                Typed = Typed.Substring(0, Typed.Length - 1);
                UpdateHighlights();
            }
        }
    }



    private void HandleInputForShop()
    {
        // Normalize player input (no spaces, lowercase)
        string normalizedTyped = Typed.ToLower();

        // Check powerup matches
        List<string> matches = _shopTrie.Autocomplete(normalizedTyped);

        // --- CASE 1: unique match -> user typed a full powerup name ---
        if (matches.Count == 1)
        {
            string matchedName = matches[0];
            CardController card = ShopManager.Instance.FindCardByNormalizedName(matchedName);

            if (card == null)
                return;

            // Check if the user has fully typed the name
            if (normalizedTyped.Length == matchedName.Length)
            {
                Debug.Log("POWERUP PURCHASED: " + card.NormalisedName);
                ShopManager.Instance.Purchase(card);

                // Reset typing
                Typed = "";
                UpdateHighlights();

                return;
            }
        }

        // --- CASE 2: player typed something invalid (no card matches this prefix) ---
        if (matches.Count == 0)
        {
            Debug.Log("Shop typo ignored (no accuracy penalty).");

            if (!(normalizedTyped.StartsWith("s") || normalizedTyped.StartsWith("sk") || normalizedTyped.StartsWith("ski") || normalizedTyped.StartsWith("skip")))
            {
                // Remove the last letter typed
                Typed = Typed.Substring(0, Typed.Length - 1);
            }

            UpdateHighlights();
            return;
        }

        // --- CASE 3: multiple matches => continue waiting ---
        // Do nothing (highlights already updated)
    }

    private void HandleInputForShopSkip()
    {
        // if player typed "skip" fully, close shop
        if (Typed == "skip")
        {
            Debug.Log("SHOP SKIPPED BY PLAYER.");
            ShopManager.Instance.DisableShop();

            // Reset typing
            Typed = "";
            UpdateHighlights();
        }
    }


    private int CountCorrectPrefix(string typed, string target)
    {
        int count = 0;
        for (int i = 0; i < typed.Length && i < target.Length; i++)
        {
            if (typed[i] == target[i]) count++;
            else break;
        }
        return count;
    }

    private void UpdateHighlights()
    {
        foreach (Enemy enemy in Spawner.Instance.Enemies)
        {
            string word = enemy.CurrentWord;

            if (string.IsNullOrEmpty(word))
            {
                enemy.ResetHighlight();
                continue;
            }

            int prefix = CountCorrectPrefix(Typed, word);
            if (prefix == Typed.Length)
            {
                enemy.HighlightPrefix(prefix);
            }
            else
            {
                enemy.ResetHighlight();
            }
        }

        foreach (Coin coin in Spawner.Instance.Coins)
        {
            string word = coin.Word;

            if (string.IsNullOrEmpty(word))
            {
                coin.ResetHighlight();
                continue;
            }

            int prefix = CountCorrectPrefix(Typed, word);
            if (prefix == Typed.Length)
                coin.HighlightPrefix(prefix);
            else
                coin.ResetHighlight();
        }


        if (ShopManager.Instance.IsShopOpen)
        {
            foreach (CardController card in ShopManager.Instance.CardControllers)
            {
                string word = card.NormalisedName;

                if (string.IsNullOrEmpty(word))
                {
                    card.ResetHighlight();
                    continue;
                }

                int prefix = CountCorrectPrefix(Typed.ToLower(), word);
                if (prefix == Typed.Length)
                {
                    card.HighlightPrefix(prefix);
                }
                else
                {
                    card.ResetHighlight();
                }
            }

            // highlight skip button
            string skipWord = "skip";
            int skipPrefix = CountCorrectPrefix(Typed.ToLower(), skipWord);
            if (skipPrefix == Typed.Length)
            {
                ShopManager.Instance.HighlightPrefix(skipPrefix);
            }
            else
            {
                ShopManager.Instance.ResetHighlight();
            }
        }
        else
        {
            foreach (var slot in InventoryManager.Instance.GetComponentsInChildren<InventorySlotController>())
            {
                if (slot.IsEmpty)
                {
                    slot.ResetHighlight();
                    continue;
                }

                string word = slot.Name;
                int prefix = CountCorrectPrefix(Typed, word);

                if (prefix == Typed.Length)
                    slot.HighlightPrefix(prefix);
                else
                    slot.ResetHighlight();
            }
        }
    }

}
