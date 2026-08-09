using Unity.Cinemachine;
using UnityEngine;

public class TestMode : MonoBehaviour
{
    public bool IsTestMode = false;
    public bool ShowGame = true;
    [SerializeField] private CinemachineCamera _testCamera;
    [SerializeField] private GameObject _screen;
    [SerializeField] private PlayerController _playerController;

    private void Start()
    {
        _testCamera.Priority = IsTestMode ? 20 : -1;
        _screen.SetActive(IsTestMode);
        _playerController.SetTestMode(IsTestMode);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (ShowGame)
        {
            BrowserController.Instance.ShowGame();
        }
    }
}
