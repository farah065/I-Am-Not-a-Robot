using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    public Trie Trie => _trie;
    public Transform PlayerPosition;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _totalEnemiesToSpawn = 20;
    private List<string> _words;
    private List<string> _usedWords;
    private Trie _trie;
    private Vector3[] _spawnPositions;
    public float SpawnRadius = 5f;
    public List<Enemy> Enemies;

    private void Start()
    {
        FillWordsListFromFile();
        _usedWords = new List<string>();
        _trie = new Trie();
        Enemies = new List<Enemy>();

        // spawn positions in 8 directions (N, NE, E, SE, S, SW, W, NW)
        _spawnPositions = new Vector3[]
        {
            new Vector3(0, SpawnRadius, 0), // N
            new Vector3(SpawnRadius / Mathf.Sqrt(2), SpawnRadius / Mathf.Sqrt(2), 0), // NE
            new Vector3(SpawnRadius, 0, 0), // E
            new Vector3(SpawnRadius / Mathf.Sqrt(2), -SpawnRadius / Mathf.Sqrt(2), 0), // SE
            new Vector3(0, -SpawnRadius, 0), // S
            new Vector3(-SpawnRadius / Mathf.Sqrt(2), -SpawnRadius / Mathf.Sqrt(2), 0), // SW
            new Vector3(-SpawnRadius, 0, 0), // W
            new Vector3(-SpawnRadius / Mathf.Sqrt(2), SpawnRadius / Mathf.Sqrt(2), 0)  // NW
        };

        StartCoroutine(SpawnCoroutine());
    }

    public Enemy FindEnemyByWord(string word)
    {
        foreach (var e in Enemies)
        {
            if (e.CurrentWord == word)
            {
                return e;
            }
        }
        return null;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        Enemies.Remove(enemy);
    }

    private IEnumerator SpawnCoroutine()
    {
        while (_totalEnemiesToSpawn > 0)
        {
            SpawnEnemy();
            _totalEnemiesToSpawn--;
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    public void RemoveWordFromTrie(string word)
    {
        _trie.Delete(word);
    }

    public void AddWordToTrie(string word)
    {
        _trie.Insert(word);
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = _spawnPositions[Random.Range(0, _spawnPositions.Length)] + PlayerPosition.position;
        GameObject enemyObject = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        AssignWordsToEnemy(enemy);
        Enemies.Add(enemy);
    }

    private void AssignWordsToEnemy(Enemy enemy)
    {
        int lettersNeeded = Mathf.CeilToInt(enemy.Hp);
        List<string> enemyWords = new List<string>();
        while (lettersNeeded > 0)
        {
            string word = "";
            do
            {
                word = _words[Random.Range(0, _words.Count)];
            } while (_usedWords.Contains(word));

            enemyWords.Add(word);
            _usedWords.Add(word);
            lettersNeeded -= word.Length;
        }

        enemy.SetWords(enemyWords);
    }

    private void FillWordsListFromFile()
    {
        TextAsset wordsFile = Resources.Load<TextAsset>("words");
        if (wordsFile != null)
        {
            _words = new List<string>();
            string[] lines = wordsFile.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                _words.Add(line.Trim());
            }
        }
        else
        {
            Debug.LogError("Words file not found.");
        }
    }
}
