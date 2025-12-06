using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;

public class TypingManager : Singleton<TypingManager>
{
    public string Typed = "";
    [SerializeField] private Player2D _player;
    private Enemy _currentTarget = null;
    private float _accuracyMultiplier = 1f;

    private Trie _enemyTrie => Spawner.Instance.Trie;
    private Trie _shopTrie => ShopManager.Instance.Trie;

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
        }
        else
        {
            HandleInputForShop();
        }
    }

    private void HandleInputForEnemies()
    {
        if (_currentTarget == null)
        {
            // PHASE 1: no target selected → use trie
            List<string> matches = _enemyTrie.Autocomplete(Typed);

            if (matches.Count == 1)
            {
                // Unique match found
                _currentTarget = Spawner.Instance.FindEnemyByWord(matches[0]);
                Debug.Log("TARGET LOCKED: " + matches[0]);
                return;
            }
            else if (matches.Count == 0)
            {
                Debug.Log("Wrong letter — no prefix matches.");
                if (!ShopManager.Instance.IsShopOpen)
                {
                    _accuracyMultiplier = 1f;
                }
                Typed = Typed.Substring(0, Typed.Length - 1);
                UpdateHighlights();
            }
        }
        else
        {
            // PHASE 2: target already selected
            string targetWord = _currentTarget.CurrentWord;

            int correctPrefix = CountCorrectPrefix(Typed, targetWord);

            if (correctPrefix == Typed.Length)
            {
                // Good letter
                if (Typed.Length == targetWord.Length)
                {
                    Debug.Log("WORD COMPLETE: " + targetWord);
                    _player.FireBullet(_currentTarget.transform.position, targetWord.Length, _accuracyMultiplier, _currentTarget);

                    Typed = "";
                    _currentTarget = null;
                }
            }
            else
            {
                Debug.Log("Wrong letter!");
                if (!ShopManager.Instance.IsShopOpen)
                {
                    _accuracyMultiplier = 1f;
                }
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

            // Remove the last letter typed
            Typed = Typed.Substring(0, Typed.Length - 1);

            UpdateHighlights();
            return;
        }

        // --- CASE 3: multiple matches => continue waiting ---
        // Do nothing (highlights already updated)
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
        }
    }

}
