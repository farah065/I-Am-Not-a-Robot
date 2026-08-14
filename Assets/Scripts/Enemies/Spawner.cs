using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Singleton<Spawner>
{
    public Trie EnemyTrie => _enemyTrie;
    public Trie CoinTrie => _coinTrie;
    public Transform PlayerPosition;
    public int TotalEnemiesToSpawn = 3;
    public float SpawnInterval = 3f;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Transform _enemyParent;
    [SerializeField] private Transform _coinParent;
    [SerializeField] private EnemyData _enemyData;
    private List<string> _words;
    private List<string> _usedWords;
    private Trie _enemyTrie;
    private Trie _coinTrie;
    public List<Enemy> Enemies;
    public List<Coin> Coins;
    private static readonly char[] Symbols = { '-', '.', ',', '!', '?' };

    private void Start()
    {
        //Initialise();
    }

    public void Initialise()
    {
        TotalEnemiesToSpawn = 3;
        SpawnInterval = 3f;

        FillWordsListFromFile();
        _usedWords = new List<string>();

        _enemyTrie = new Trie();
        _coinTrie = new Trie();

        Enemies = new List<Enemy>();
        Coins = new List<Coin>();

        foreach (Transform child in _enemyParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in _coinParent)
        {
            Destroy(child.gameObject);
        }

        StartWave();
    }

    public void StartWave()
    {
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
        RemoveWordFromTrie(enemy.CurrentWord);
    }

    private IEnumerator SpawnCoroutine()
    {
        int enemiesToSpawn = TotalEnemiesToSpawn;
        while (enemiesToSpawn > 0)
        {
            SpawnEnemy();
            enemiesToSpawn--;
            yield return new WaitForSeconds(SpawnInterval);
        }

        while (Enemies.Count > 0)
        {
            yield return null;
        }

        if (Player2D.Instance.Hp > 0)
        {
            ClearAllCoins();

            yield return new WaitForSeconds(1f);
            StartCoroutine(GameManager.Instance.OnWaveEnd());
        }
    }

    public void RemoveWordFromTrie(string word)
    {
        _enemyTrie.Delete(word);
    }

    public void AddWordToTrie(string word)
    {
        _enemyTrie.Insert(word);
        //_enemyTrie.PrintAllWords();
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
        GameObject enemyObject = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
        enemyObject.transform.SetParent(_enemyParent);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        AssignWordsToEnemy(enemy);
        Enemies.Add(enemy);
    }

    public void SpawnCoin(Enemy enemy)
    {
        Vector3 spawnPosition = enemy.transform.position;
        GameObject coinObject = Instantiate(_coinPrefab, spawnPosition, Quaternion.identity);
        coinObject.transform.SetParent(_coinParent);
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
        int lettersNeeded = Mathf.CeilToInt(_enemyData.MaxHp);
        List<string> enemyWords = new List<string>();
        while (lettersNeeded > 0)
        {
            string baseWord;
            do
            {
                baseWord = _words[Random.Range(0, _words.Count)];
            }
            while (_usedWords.Contains(baseWord));

            // Apply modifiers BEFORE storing
            string finalWord = ApplyAreaModifiers(baseWord);

            enemyWords.Add(finalWord);
            _usedWords.Add(baseWord);

            lettersNeeded -= finalWord.Length;
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

    public string ApplyAreaModifiers(string baseWord)
    {
        string result = baseWord;
        Area area = GameManager.Instance.CurrentArea;

        // Always: chance for NO modification at all
        if (Random.value < 0.50f)
            return result; // 50% base chance to stay normal

        switch (area)
        {
            case Area.Mountain:
                return ApplyMountain(result);

            case Area.Cave:
                return ApplyCave(result);

            case Area.Core:
                return ApplyCore(result);

            default:
                return result;
        }
    }

    private string ApplyMountain(string word)
    {
        // 40% chance first letter becomes capitalized
        if (Random.value < 0.40f)
            return Capitalize(word);

        return word; // unchanged
    }

    private string ApplyCave(string word)
    {
        float r = Random.value;

        if (r < 0.33f)
        {
            // 1/3 chance — Capitalize first letter
            return Capitalize(word);
        }
        else if (r < 0.66f)
        {
            // 1/3 chance — Add symbol at end
            return AddSymbol(word);
        }

        // 1/3 chance — stay normal
        return word;
    }

    private string ApplyCore(string word)
    {
        float r = Random.value;

        if (r < 0.25f)
        {
            // 25% just capitalization
            return Capitalize(word);
        }
        else if (r < 0.50f)
        {
            // 25% just symbol
            return AddSymbol(word);
        }
        else if (r < 0.60f)
        {
            // 10% BOTH happen at once
            return AddSymbol(Capitalize(word));
        }

        // 40% unchanged
        return word;
    }

    private string Capitalize(string w)
    {
        if (string.IsNullOrEmpty(w)) return w;
        return char.ToUpper(w[0]) + w.Substring(1);
    }

    private string AddSymbol(string w)
    {
        return w + Symbols[Random.Range(0, Symbols.Length)];
    }
}
