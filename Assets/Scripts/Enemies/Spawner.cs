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
    [SerializeField] private Transform[] _spawnPoints;
    private List<string> _words;
    private List<string> _usedWords;
    private Trie _trie;
    public List<Enemy> Enemies;

    private void Start()
    {
        FillWordsListFromFile();
        _usedWords = new List<string>();
        _trie = new Trie();
        Enemies = new List<Enemy>();

        StartWave();
    }

    public void StartWave()
    {
        _totalEnemiesToSpawn = 2;
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

        while (Enemies.Count > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(GameManager.Instance.OnWaveEnd());
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
        Vector3 spawnPosition = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
        GameObject enemyObject = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        AssignWordsToEnemy(enemy);
        Enemies.Add(enemy);
    }

    private void AssignWordsToEnemy(Enemy enemy)
    {
        int lettersNeeded = Mathf.CeilToInt(enemy.CurrentHp);
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
