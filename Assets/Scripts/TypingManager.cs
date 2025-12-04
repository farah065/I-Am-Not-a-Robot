using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;

public class TypingManager : MonoBehaviour
{
    public string Typed = "";
    private Enemy _currentTarget = null;
    private float _accuracyMultiplier = 1f;

    private Trie _trie => Spawner.Instance.Trie;

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
        UpdateEnemyHighlights();

        if (_currentTarget == null)
        {
            // PHASE 1: no target selected → use trie
            List<string> matches = _trie.Autocomplete(Typed);

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
                _accuracyMultiplier = 1f;
                Typed = Typed.Substring(0, Typed.Length - 1);
                UpdateEnemyHighlights();
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
                    _currentTarget.TakeDamage(targetWord.Length, _accuracyMultiplier);

                    Typed = "";
                    _currentTarget = null;
                }
            }
            else
            {
                Debug.Log("Wrong letter!");
                _accuracyMultiplier = 1f;
                Typed = Typed.Substring(0, Typed.Length - 1);
                UpdateEnemyHighlights();
            }
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

    private void UpdateEnemyHighlights()
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
                enemy.HighlightPrefix(prefix);
            else
                enemy.ResetHighlight();
        }
    }

}
