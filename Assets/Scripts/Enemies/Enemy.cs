using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float CurrentHp;
    public EnemyData EnemyData;
    public string CurrentWord = "";
    public float FrozenTimer = 0f;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private TMP_Text _currentWordText;
    [SerializeField] private List<string> _words = new List<string>();
    [SerializeField] private Transform _centrePoint;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private Image _hpBar;

    private float _currentSpeed;
    private Vector3 _direction;

    private void Start()
    {
        _direction = (Spawner.Instance.PlayerPosition.position - _centrePoint.position).normalized;

        CurrentHp = EnemyData.MaxHp;
        _currentSpeed = EnemyData.BaseSpeed;

        _hpText.text = CurrentHp.ToString();
    }

    private void FixedUpdate()
    {
        if (Player2D.Instance.Hp > 0)
        {
            if (FrozenTimer > 0f)
            {
                FrozenTimer -= Time.fixedDeltaTime;
                _rb.linearVelocity = Vector2.zero;
            }
            else
            {
                _direction = (Spawner.Instance.PlayerPosition.position - _centrePoint.position).normalized;
                _rb.linearVelocity = _direction * _currentSpeed;
            }
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    public void SetWords(List<string> words)
    {
        _words = words;
        if (_words.Count > 0)
        {
            SetCurrentWord(_words[0]);
        }
    }

    public void TakeDamage(float damage, float multiplier)
    {
        CurrentHp -= damage * multiplier;

        // Round HP to one decimal place
        CurrentHp = Mathf.Round(CurrentHp * 10f) / 10f;
        _hpText.text = CurrentHp.ToString();

        float hpPercentage = CurrentHp / EnemyData.MaxHp;
        RectTransform hpBarRect = _hpBar.GetComponent<RectTransform>();

        float newWidth = hpBarRect.rect.width * hpPercentage;
        hpBarRect.sizeDelta = new Vector2(newWidth, hpBarRect.sizeDelta.y);

        UpdateCurrentWord();

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    public void HighlightPrefix(int length)
    {
        if (string.IsNullOrEmpty(CurrentWord))
            return;

        length = Mathf.Clamp(length, 0, CurrentWord.Length);

        string prefix = CurrentWord.Substring(0, length);
        string suffix = CurrentWord.Substring(length);

        // Highlight prefix in green
        string coloredPrefix = $"<color=#86C06C>{prefix}</color>";

        _currentWordText.text = coloredPrefix + suffix;
    }

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

    private void SetCurrentWord(string word)
    {
        CurrentWord = word;
        _currentWordText.text = CurrentWord;

        if (word != null && word.Length > 0 && CurrentHp > 0)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player2D>().TakeDamage();

            TypingManager.Instance.ResetTypedIfMatch(CurrentWord);

            Spawner.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }
}
