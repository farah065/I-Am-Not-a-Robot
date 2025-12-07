using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    public Trie EnemyTrie => _enemyTrie;
    public Trie CoinTrie => _coinTrie;
    public Transform PlayerPosition;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private int _totalEnemiesToSpawn = 20;
    [SerializeField] private Transform[] _spawnPoints;
    private List<string> _words;
    private List<string> _usedWords;
    private Trie _enemyTrie;
    private Trie _coinTrie;
    public List<Enemy> Enemies;
    public List<Coin> Coins;

    private void Start()
    {
        FillWordsListFromFile();
        _usedWords = new List<string>();

        _enemyTrie = new Trie();
        Enemies = new List<Enemy>();

        _coinTrie = new Trie();
        Coins = new List<Coin>();

        StartWave();
    }

    public void StartWave()
    {
        _totalEnemiesToSpawn = 3;
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
        ClearAllCoins();

        yield return new WaitForSeconds(1f);
        StartCoroutine(GameManager.Instance.OnWaveEnd());
    }

    public void RemoveWordFromTrie(string word)
    {
        _enemyTrie.Delete(word);
    }

    public void AddWordToTrie(string word)
    {
        _enemyTrie.Insert(word);
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
        GameObject enemyObject = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        AssignWordsToEnemy(enemy);
        Enemies.Add(enemy);
    }

    public void SpawnCoin(Enemy enemy)
    {
        Vector3 spawnPosition = enemy.transform.position;
        GameObject coinObject = Instantiate(_coinPrefab, spawnPosition, Quaternion.identity);
        Coin coin = coinObject.GetComponent<Coin>();

        // get random unused word
        string word = "";
        do
        {
            word = _words[Random.Range(0, _words.Count)];
        } while (_usedWords.Contains(word));

        coin.SetWord(word);
        _coinTrie.Insert(word);
        _usedWords.Add(word);
        Coins.Add(coin);
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

    private void ClearAllCoins()
    {
        foreach (var coin in Coins)
        {
            Destroy(coin.gameObject);
        }
        _coinTrie = new Trie();
        Coins.Clear();
    }

    public Coin FindCoinByWord(string word)
    {
        foreach (var c in Coins)
        {
            if (c.Word == word)
            {
                return c;
            }
        }
        return null;
    }

    public void CollectCoin(Coin coin)
    {
        _coinTrie.Delete(coin.Word);
        Coins.Remove(coin);
        Destroy(coin.gameObject);
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
