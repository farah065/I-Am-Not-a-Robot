using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float CurrentHp = 30f;
    public EnemyData _enemyData;
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
        CurrentHp = _enemyData.MaxHp;
        _currentSpeed = _enemyData.BaseSpeed;
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
                float distance = Vector3.Distance(Spawner.Instance.PlayerPosition.position, _centrePoint.position);
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
        // round hp to 1 decimal place
        CurrentHp = Mathf.Round(CurrentHp * 10f) / 10f;
        _hpText.text = CurrentHp.ToString();

        // Update HP bar width based on current HP percentage
        float hpPercentage = CurrentHp / _enemyData.MaxHp;
        RectTransform hpBarRect = _hpBar.GetComponent<RectTransform>();

        // Set the new width of the hpBar
        float newWidth = hpBarRect.rect.width * hpPercentage; // Adjust the width based on the percentage
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

        // Change this color to whatever highlight you want
        string coloredPrefix = $"<color=#86C06C>{prefix}</color>";

        _currentWordText.text = coloredPrefix + suffix;
    }

    public void ResetHighlight()
    {
        _currentWordText.text = CurrentWord;
    }

    private void UpdateCurrentWord()
    {
        Spawner.Instance.RemoveWordFromTrie(CurrentWord);
        _words.RemoveAt(0);
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
        if (word != null && word.Length > 0 && word != "")
        {
            Spawner.Instance.AddWordToTrie(CurrentWord);
        }
    }

    private void Die()
    {
        if (Random.value < _enemyData.BaseCoinDropChance && Spawner.Instance.Coins.Count < 3)
        {
            Spawner.Instance.SpawnCoin(this);
        }
        Spawner.Instance.RemoveEnemy(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player2D>().TakeDamage();
            Spawner.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }
}
