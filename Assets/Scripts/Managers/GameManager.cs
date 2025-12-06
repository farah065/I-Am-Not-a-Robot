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

    private void Start()
    {
        _currentWave = 1;
        MoveToArea(Area.Forest);
    }

    private void IncrementWave()
    {
        if (_currentWave == _areaTransitionPoints[0])
        {
            MoveToArea(Area.Mountain);
        }
        else if (_currentWave == _areaTransitionPoints[1])
        {
            MoveToArea(Area.Cave);
        }
        else if (_currentWave == _areaTransitionPoints[2])
        {
            MoveToArea(Area.Core);
        }

        _currentWave++;
    }

    private void MoveToArea(Area area)
    {
        Debug.Log("Moving to area: " + area.ToString());
        _currentArea = area;

        for (int i = 0; i < _areaTilemaps.Length; i++)
        {
            _areaTilemaps[i].SetActive(i == (int)area);
        }
    }

    public IEnumerator OnWaveEnd()
    {
        // Show wave end UI
        _waveEndText.text = "Wave " + _currentWave + " Complete!";
        _waveEndUI.SetActive(true);

        // Wait for a few seconds
        yield return new WaitForSeconds(3f);

        // Hide wave end UI
        _waveEndUI.SetActive(false);

        // show shop
        ShopManager.Instance.EnableShop();
    }

    private IEnumerator OnShopClosed()
    {
        yield return new WaitForSeconds(1f);
    }
}
