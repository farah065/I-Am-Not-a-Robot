using UnityEngine;
using TMPro;
using System.Collections;

public enum Area
{
    Forest,
    Mountain,
    Cave,
    Core
}

public class GameManager : Singleton<GameManager>
{
    public Area CurrentArea;

    [SerializeField] private int _currentWave;
    [SerializeField] private int[] _areaTransitionPoints;
    [SerializeField] private GameObject[] _areaTilemaps;
    [SerializeField] private GameObject _waveEndUI;
    [SerializeField] private TMP_Text _waveEndText;
    [SerializeField] private Player2D _player;
    [SerializeField] private EnemyData _enemyData;

    public void WaveScaleUp()
    {
        if (_currentWave % 3 == 0)
        {
            _enemyData.MaxHp = Mathf.Min(24, _enemyData.MaxHp + 2);
            _enemyData.BaseSpeed += 0.1f;
        }

        Spawner.Instance.TotalEnemiesToSpawn = Mathf.Min(30, Spawner.Instance.TotalEnemiesToSpawn + 1);
    }

    private void OnEnable()
    {
        InitialiseGame();
    }

    public void InitialiseGame()
    {
        _currentWave = 1;

        CurrentArea = Area.Forest;

        Player2D.Instance.Initialise();
        InventoryManager.Instance.ClearInventory();
        TypingManager.Instance.Initialise();
        Spawner.Instance.Initialise();

        for (int i = 0; i < _areaTilemaps.Length; i++)
        {
            _areaTilemaps[i].SetActive(i == (int)CurrentArea);
        }

        _enemyData.MaxHp = 10;
        _enemyData.BaseSpeed = 0.5f;
    }

    private void IncrementWave()
    {
        if (_currentWave == _areaTransitionPoints[0])
        {
            _currentWave++;
            StartCoroutine(MoveToArea(Area.Mountain));
        }
        else if (_currentWave == _areaTransitionPoints[1])
        {
            _currentWave++;
            StartCoroutine(MoveToArea(Area.Cave));
        }
        else if (_currentWave == _areaTransitionPoints[2])
        {
            _currentWave++;
            StartCoroutine(MoveToArea(Area.Core));
        }
        else
        {
            _currentWave++;
            TypingManager.Instance.Typed = "";
            WaveScaleUp();
            Spawner.Instance.StartWave();
        }
    }

    private IEnumerator MoveToArea(Area area)
    {
        _player.PlaySceneTransitionAnimation();

        yield return new WaitUntil(() => _player.HasReachedExit);

        CurrentArea = area;

        for (int i = 0; i < _areaTilemaps.Length; i++)
        {
            _areaTilemaps[i].SetActive(i == (int)area);
        }

        yield return new WaitUntil(() => !_player.IsTransitioning);
        yield return new WaitForSeconds(1f);

        _waveEndText.text = "The " + area.ToString();
        _waveEndUI.SetActive(true);

        yield return new WaitForSeconds(3f);

        _waveEndUI.SetActive(false);

        Spawner.Instance.StartWave();
    }

    public IEnumerator OnWaveEnd()
    {
        Player2D.Instance.ResetPowerupEffects();

        _waveEndText.text = "Wave " + _currentWave + " Complete!";
        _waveEndUI.SetActive(true);

        yield return new WaitForSeconds(3f);

        _waveEndUI.SetActive(false);

        if (!InventoryManager.Instance.IsInventoryFull() && CurrentArea != Area.Core)
        {
            ShopManager.Instance.EnableShop();
        }
        else
        {
            yield return OnShopClosed();
        }
    }

    public IEnumerator OnShopClosed()
    {
        yield return new WaitForSeconds(1f);
        IncrementWave();
    }
}
