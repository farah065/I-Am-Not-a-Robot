using UnityEngine;

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

    private void Start()
    {
        _currentWave = 1;
        _currentArea = Area.Forest;
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
    }
}
