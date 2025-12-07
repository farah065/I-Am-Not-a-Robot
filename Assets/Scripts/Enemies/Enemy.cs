using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    // Current HP value that decreases when the enemy takes damage
    public float CurrentHp;

    // Scriptable object containing stats and configuration for this enemy type
    public EnemyData EnemyData;

    // The current word the player must type to damage this enemy
    public string CurrentWord = "";

    // Timer tracking how long the enemy remains frozen
    public float FrozenTimer = 0f;

    // Rigidbody2D used for movement
    [SerializeField] private Rigidbody2D _rb;

    // UI text showing the current target word
    [SerializeField] private TMP_Text _currentWordText;

    // List of words assigned to this enemy (one is active at a time)
    [SerializeField] private List<string> _words = new List<string>();

    // A reference point on the enemy used for direction calculations
    [SerializeField] private Transform _centrePoint;

    // UI showing HP value
    [SerializeField] private TMP_Text _hpText;

    // UI component representing the HP bar
    [SerializeField] private Image _hpBar;

    // Current movement speed (may be modified by enemy stats)
    private float _currentSpeed;

    // Movement direction toward the player
    private Vector3 _direction;

    private void Start()
    {
        // Determine initial direction toward the player
        _direction = (Spawner.Instance.PlayerPosition.position - _centrePoint.position).normalized;

        // Initialize HP and speed
        CurrentHp = EnemyData.MaxHp;
        _currentSpeed = EnemyData.BaseSpeed;

        // Display initial HP in UI
        _hpText.text = CurrentHp.ToString();
    }

    private void FixedUpdate()
    {
        // Only move if the player is alive
        if (Player2D.Instance.Hp > 0)
        {
            // If frozen, count down and prevent movement
            if (FrozenTimer > 0f)
            {
                FrozenTimer -= Time.fixedDeltaTime;
                _rb.linearVelocity = Vector2.zero;
            }
            else
            {
                // Update direction toward the player
                _direction = (Spawner.Instance.PlayerPosition.position - _centrePoint.position).normalized;

                // Move toward the player
                float distance = Vector3.Distance(Spawner.Instance.PlayerPosition.position, _centrePoint.position);
                _rb.linearVelocity = _direction * _currentSpeed;
            }
        }
        else
        {
            // Player dead — stop moving
            _rb.linearVelocity = Vector2.zero;
        }
    }

    // Assigns a list of words this enemy will cycle through
    public void SetWords(List<string> words)
    {
        _words = words;
        if (_words.Count > 0)
        {
            SetCurrentWord(_words[0]);
        }
    }

    // Reduces HP and updates UI and states accordingly
    public void TakeDamage(float damage, float multiplier)
    {
        // Apply damage with multiplier
        CurrentHp -= damage * multiplier;

        // Round HP to one decimal place
        CurrentHp = Mathf.Round(CurrentHp * 10f) / 10f;
        _hpText.text = CurrentHp.ToString();

        // Update HP bar based on remaining percentage
        float hpPercentage = CurrentHp / EnemyData.MaxHp;
        RectTransform hpBarRect = _hpBar.GetComponent<RectTransform>();

        // Adjust bar width proportionally
        float newWidth = hpBarRect.rect.width * hpPercentage;
        hpBarRect.sizeDelta = new Vector2(newWidth, hpBarRect.sizeDelta.y);

        // Switch to next word or clear it
        UpdateCurrentWord();

        // Handle death if HP reached zero
        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    // Highlights the typed prefix of the current word
    public void HighlightPrefix(int length)
    {
        if (string.IsNullOrEmpty(CurrentWord))
            return;

        // Ensure highlight length is within valid range
        length = Mathf.Clamp(length, 0, CurrentWord.Length);

        string prefix = CurrentWord.Substring(0, length);
        string suffix = CurrentWord.Substring(length);

        // Highlight prefix in green
        string coloredPrefix = $"<color=#86C06C>{prefix}</color>";

        _currentWordText.text = coloredPrefix + suffix;
    }

    // Resets the word display to normal without highlight
    public void ResetHighlight()
    {
        _currentWordText.text = CurrentWord;
    }

    // Removes the current word and moves to the next one
    private void UpdateCurrentWord()
    {
        // Remove word from Trie for typing system
        Spawner.Instance.RemoveWordFromTrie(CurrentWord);

        // Remove used word from internal list
        _words.RemoveAt(0);

        // Set next word if available, otherwise clear
        if (_words.Count > 0)
        {
            SetCurrentWord(_words[0]);
        }
        else
        {
            SetCurrentWord("");
        }
    }

    // Sets the currently active word and updates UI + Trie
    private void SetCurrentWord(string word)
    {
        CurrentWord = word;
        _currentWordText.text = CurrentWord;

        // Add word to Trie only if valid and not empty
        if (word != null && word.Length > 0 && word != "")
        {
            Spawner.Instance.AddWordToTrie(CurrentWord);
        }
    }

    // Handles enemy death, drops coins, and removes enemy from the scene
    private void Die()
    {
        // Random chance to drop a coin if total coins in scene < 3 and area allows it
        if (Random.value < EnemyData.BaseCoinDropChance && Spawner.Instance.Coins.Count < 3 && GameManager.Instance.CurrentArea != Area.Core)
        {
            Spawner.Instance.SpawnCoin(this);
        }

        // Remove enemy from active enemies list
        Spawner.Instance.RemoveEnemy(this);

        // Destroy enemy object
        Destroy(gameObject);
    }

    // Handles collision with the player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Damage the player on contact
            collision.GetComponent<Player2D>().TakeDamage();

            // Remove enemy and destroy object
            Spawner.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }
}
