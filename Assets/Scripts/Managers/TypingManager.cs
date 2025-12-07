using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;
using TMPro;

public class TypingManager : Singleton<TypingManager>
{
    public string Typed = "";
    [SerializeField] private Player2D _player;
    [SerializeField] private TMP_Text _accuracyMultiplierText;

    private float _accuracyMultiplier = 1f;
    private Enemy _currentTarget = null;
    private Coin _currentCoinTarget = null;
    private PowerupData _currentInventoryTarget = null;

    private Trie _enemyTrie => Spawner.Instance.EnemyTrie;
    private Trie _shopTrie => ShopManager.Instance.Trie;
    private Trie _coinTrie => Spawner.Instance.CoinTrie;
    private Trie _inventoryTrie => InventoryManager.Instance.InventoryTrie;

    public void Initialise()
    {
        Typed = "";
        ResetAccuracyMultiplier();
        _currentTarget = null;
        _currentCoinTarget = null;
        _currentInventoryTarget = null;
    }

    private void Update()
    {
        // check every key on the keyboard
        foreach (var key in Keyboard.current.allKeys)
        {
            if (key == null) continue;
            // if it was pressed this frame
            if (key.wasPressedThisFrame)
            {
                // get the character and handle input
                char c = ConvertKey(key);
                if (c != '\0') HandleInput(c);
            }
        }
    }

    private char ConvertKey(KeyControl key)
    {
        // Reject modifier keys
        if (key.keyCode == Key.None) return '\0';

        // Check if shift is held
        bool shift = Keyboard.current.shiftKey.isPressed;

        // Check if caps lock is enabled (using the LED state)
        bool capsLock = Keyboard.current.capsLockKey.isPressed;

        // Handle letters (A-Z keys)
        if (key.keyCode >= Key.A && key.keyCode <= Key.Z)
        {
            char baseLetter = (char)('a' + (key.keyCode - Key.A));

            // Determine final casing: XOR → capital only when ONE is active
            bool makeUpper = shift ^ capsLock;

            return makeUpper ? char.ToUpper(baseLetter) : baseLetter;
        }

        // Handle allowed symbols
        // Period
        if (key.keyCode == Key.Period) return '.';
        // Comma
        if (key.keyCode == Key.Comma) return ',';
        // Minus/Hyphen
        if (key.keyCode == Key.Minus) return '-';

        // Shift-dependent symbols
        if (shift)
        {
            // Exclamation mark (Shift + 1)
            if (key.keyCode == Key.Digit1) return '!';
            // Question mark (Shift + /)
            if (key.keyCode == Key.Slash) return '?';
        }

        return '\0';
    }


    private void HandleInput(char c)
    {
        // Append character to typed string
        Typed += c;
        UpdateHighlights();

        if (!ShopManager.Instance.IsShopOpen)
        {
            // if the shop isnt open, handle enemy/coin/inventory input
            HandleInputForRound();
        }
        else
        {
            // otherwise, handle shop input (cards and skip button)
            HandleInputForShop();
            HandleInputForShopSkip();
        }
    }

    private void HandleInputForRound()
    {
        // if no target yet
        if (_currentTarget == null && _currentCoinTarget == null && _currentInventoryTarget == null)
        {
            List<string> enemyMatches = _enemyTrie.Autocomplete(Typed);
            List<string> coinMatches = _coinTrie.Autocomplete(Typed);
            List<string> inventoryMatches = _inventoryTrie.Autocomplete(Typed);

            // if exactly one enemy/coin/inventory slot matches, lock it as a target
            if (enemyMatches.Count == 1 && coinMatches.Count == 0 && inventoryMatches.Count == 0)
            {
                _currentTarget = Spawner.Instance.FindEnemyByWord(enemyMatches[0]);
                return;
            }
            else if (coinMatches.Count == 1 && enemyMatches.Count == 0 && inventoryMatches.Count == 0)
            {
                _currentCoinTarget = Spawner.Instance.FindCoinByWord(coinMatches[0]);
                return;
            }
            else if (inventoryMatches.Count == 1 && enemyMatches.Count == 0 && coinMatches.Count == 0)
            {
                string powerupName = inventoryMatches[0];
                _currentInventoryTarget = InventoryManager.Instance.FindInventoryPowerup(powerupName);
                return;
            }

            // if no matches at all, the user made a typo
            if (enemyMatches.Count == 0 && coinMatches.Count == 0 && inventoryMatches.Count == 0)
            {
                // reset accuracy multiplier
                ResetAccuracyMultiplier();

                // remove last typed character
                Typed = Typed.Substring(0, Typed.Length - 1);
                UpdateHighlights();
            }

            return;
        }

        string targetWord;
        // if enemy is targeted
        if (_currentTarget != null)
        {
            targetWord = _currentTarget.CurrentWord;
        }
        else if (_currentCoinTarget != null)
        {
            targetWord = _currentCoinTarget.Word;
        }
        else if (_currentInventoryTarget != null)
        {
            targetWord = _currentInventoryTarget.TypableName;
        }
        else
        {
            return;
        }

        int correctPrefix = CountCorrectPrefix(Typed, targetWord);

        // if the number of correct letters equals the typed length so far, we havent made a typo yet
        if (correctPrefix == Typed.Length)
        {
            // if the number of correct letters equals the target word length, we finished typing the word
            if (correctPrefix == targetWord.Length)
            {
                // if enemy targeted
                if (_currentTarget != null)
                {
                    // fire a bullet towards the enemy who had the word
                    _player.FireBullet(_currentTarget.transform.position, targetWord.Length, _accuracyMultiplier, _currentTarget);
                    // increase accuracy multiplier
                    float newMult = Mathf.Min(_accuracyMultiplier + 0.05f, 2f);
                    SetAccuracyMultiplier(newMult);
                }
                else if (_currentCoinTarget != null)
                {
                    Spawner.Instance.CollectCoin(_currentCoinTarget);
                    _player.AddCoins(1);
                }
                else if (_currentInventoryTarget != null)
                {
                    InventoryManager.Instance.UsePowerup(_currentInventoryTarget);
                }

                // reset typing state and target
                Typed = "";
                _currentTarget = null;
                _currentCoinTarget = null;
                _currentInventoryTarget = null;
            }
        }
        else
        {
            // typo detected, reset accuracy multiplier
            ResetAccuracyMultiplier();

            // remove last typed character
            Typed = Typed.Substring(0, Typed.Length - 1);
            UpdateHighlights();
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
            ShopManager.Instance.DisableShop();

            // Reset typing
            Typed = "";
            UpdateHighlights();
        }
    }

    private int CountCorrectPrefix(string typed, string target)
    {
        int count = 0;

        // count matching characters from start
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

        foreach (var slot in InventoryManager.Instance.InventorySlots)
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
    }

    private void SetAccuracyMultiplier(float multiplier)
    {
        _accuracyMultiplier = multiplier;
        _accuracyMultiplierText.text = "x" + _accuracyMultiplier.ToString("F2") + " dmg";
    }

    private void ResetAccuracyMultiplier()
    {
        PowerupData autocorrectPowerup = InventoryManager.Instance.FindInventoryPowerup("autocorrect");
        if (autocorrectPowerup != null)
        {
            InventoryManager.Instance.UsePowerup(autocorrectPowerup);
        }
        else
        {
            SetAccuracyMultiplier(1f);
        }
    }
}
