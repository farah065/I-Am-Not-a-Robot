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
    [SerializeField] private int _currentWave;
    [SerializeField] private Area _currentArea;
    [SerializeField] private int[] _areaTransitionPoints;
    [SerializeField] private GameObject[] _areaTilemaps;
    [SerializeField] private GameObject _waveEndUI;
    [SerializeField] private TMP_Text _waveEndText;
    [SerializeField] private Player2D _player;

    private void Start()
    {
        _currentWave = 1;

        _currentArea = Area.Forest;

        for (int i = 0; i < _areaTilemaps.Length; i++)
        {
            _areaTilemaps[i].SetActive(i == (int)_currentArea);
        }
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
            Spawner.Instance.StartWave();
        }
    }

    private IEnumerator MoveToArea(Area area)
    {
        _player.PlaySceneTransitionAnimation();
        yield return new WaitUntil(() => _player.HasReachedExit);
        _currentArea = area;

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

        // Show wave end UI
        _waveEndText.text = "Wave " + _currentWave + " Complete!";
        _waveEndUI.SetActive(true);

        // Wait for a few seconds
        yield return new WaitForSeconds(3f);

        // Hide wave end UI
        _waveEndUI.SetActive(false);

        // show shop
        if (!InventoryManager.Instance.IsInventoryFull() && _currentArea != Area.Core)
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
