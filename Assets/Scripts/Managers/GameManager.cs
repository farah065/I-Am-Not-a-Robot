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

// Main game controller handling waves, area transitions, and global initialization.
// Uses Singleton for global access.
public class GameManager : Singleton<GameManager>
{
    // The area the player is currently in
    public Area CurrentArea;

    // Current wave number
    [SerializeField] private int _currentWave;

    // Wave numbers at which area transitions occur
    [SerializeField] private int[] _areaTransitionPoints;

    // Tilemap GameObjects for each area (only one active at a time)
    [SerializeField] private GameObject[] _areaTilemaps;

    // UI displayed after wave completion
    [SerializeField] private GameObject _waveEndUI;

    // Text element displaying wave/area messages
    [SerializeField] private TMP_Text _waveEndText;

    // Reference to the player
    [SerializeField] private Player2D _player;

    // Reference to the enemy data used for scaling difficulty
    [SerializeField] private EnemyData _enemyData;

    // Scales enemy stats and increases spawn count every few waves
    public void WaveScaleUp()
    {
        // Every 3 waves, scale up enemy HP and speed
        if (_currentWave % 3 == 0)
        {
            _enemyData.MaxHp += 1f;
            _enemyData.BaseSpeed += 0.05f;
        }

        // Increment max enemies per wave, capped at 30
        Spawner.Instance.TotalEnemiesToSpawn = Mathf.Min(30, Spawner.Instance.TotalEnemiesToSpawn + 1);
    }

    private void OnEnable()
    {
        // Begin the game setup process
        InitialiseGame();
    }

    // Resets game state, enemies, UI, player, and area tiles
    public void InitialiseGame()
    {
        Debug.Log("INITIALISING GAME");
        _currentWave = 1;

        // Start in the Forest area
        CurrentArea = Area.Forest;

        // Reset core game systems
        Player2D.Instance.Initialise();
        InventoryManager.Instance.ClearInventory();
        TypingManager.Instance.Initialise();
        Spawner.Instance.Initialise();

        // Activate only the tilemap for the current area
        for (int i = 0; i < _areaTilemaps.Length; i++)
        {
            _areaTilemaps[i].SetActive(i == (int)CurrentArea);
        }

        // Reset enemy stats
        _enemyData.MaxHp = 10;
        _enemyData.BaseSpeed = 0.5f;
    }

    // Progresses to the next wave or transitions to the next area if needed
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
            // Continue normal wave progression
            _currentWave++;
            TypingManager.Instance.Typed = "";
            WaveScaleUp();
            Spawner.Instance.StartWave();
        }
    }

    // Handles player movement animation, area switching, and UI display for transitions
    private IEnumerator MoveToArea(Area area)
    {
        // Start scene transition animation
        _player.PlaySceneTransitionAnimation();

        // Wait until player reaches exit trigger
        yield return new WaitUntil(() => _player.HasReachedExit);

        // Switch current game area
        CurrentArea = area;

        // Enable tilemap for new area
        for (int i = 0; i < _areaTilemaps.Length; i++)
        {
            _areaTilemaps[i].SetActive(i == (int)area);
        }

        // Wait for transition animation to finish
        yield return new WaitUntil(() => !_player.IsTransitioning);
        yield return new WaitForSeconds(1f);

        // Show the "Entering Area" UI
        _waveEndText.text = "The " + area.ToString();
        _waveEndUI.SetActive(true);

        // Display UI for a moment
        yield return new WaitForSeconds(3f);

        _waveEndUI.SetActive(false);

        // Begin next wave automatically
        Spawner.Instance.StartWave();
    }

    // Called when a wave ends — shows UI, opens shop if needed, then continues game flow
    public IEnumerator OnWaveEnd()
    {
        // Remove temporary powerup effects
        Player2D.Instance.ResetPowerupEffects();

        // Display wave complete UI
        _waveEndText.text = "Wave " + _currentWave + " Complete!";
        _waveEndUI.SetActive(true);

        // Delay for player to read
        yield return new WaitForSeconds(3f);

        // Hide UI
        _waveEndUI.SetActive(false);

        // Open shop unless inventory full or in the Core area
        if (!InventoryManager.Instance.IsInventoryFull() && CurrentArea != Area.Core)
        {
            ShopManager.Instance.EnableShop();
        }
        else
        {
            // Skip shop and proceed
            yield return OnShopClosed();
        }
    }

    // Called after shop closes or when skipping shop; proceeds to next wave
    public IEnumerator OnShopClosed()
    {
        yield return new WaitForSeconds(1f);
        IncrementWave();
    }
}
