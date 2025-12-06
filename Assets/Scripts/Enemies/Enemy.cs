using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Enemy : MonoBehaviour
{
    public float Hp = 30f;
    public string CurrentWord = "";
    [SerializeField] private float _speed = 1f;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private TMP_Text _currentWordText;
    [SerializeField] private List<string> _words = new List<string>();
    [SerializeField] private Transform _centrePoint;
    private Vector3 _direction;

    private void Start()
    {
        _direction = (Spawner.Instance.PlayerPosition.position - _centrePoint.position).normalized;
    }

    private void FixedUpdate()
    {
        _direction = (Spawner.Instance.PlayerPosition.position - _centrePoint.position).normalized;
        float distance = Vector3.Distance(Spawner.Instance.PlayerPosition.position, _centrePoint.position);
        _rb.linearVelocity = _direction * _speed;
    }

    private void OnDrawGizmos()
    {
        // draw the distance between enemy and player
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_centrePoint.position, Spawner.Instance.PlayerPosition.position);
        // draw the direction vector
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(_centrePoint.position, _centrePoint.position + _direction * 2f);
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
        Hp -= damage * multiplier;

        UpdateCurrentWord();

        if (Hp <= 0)
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
        Spawner.Instance.RemoveEnemy(this);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Enemy reached the player!");
            Spawner.Instance.RemoveEnemy(this);
            Destroy(gameObject);
        }
    }
}
