using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class MonitorController : MonoBehaviour, IInteractable
{
    public bool IsMonitorOn = false;
    [SerializeField] private GameObject _monitorScreen;
    [SerializeField] private CinemachineCamera _deskCamera;

    public void Interact()
    {
        if (IsMonitorOn)
        {
            SwitchOffMonitor();
        }
        else
        {
            SwitchOnMonitor();
        }
    }

    private void SwitchOnMonitor()
    {
        _monitorScreen.SetActive(true);
        IsMonitorOn = true;
        _deskCamera.transform.DOLocalMove(_deskCamera.transform.localPosition + new Vector3(0, 0.4f, -2.23f), 0.5f);
        _deskCamera.transform.DOLocalRotate(_deskCamera.transform.localEulerAngles + new Vector3(8.512f, 0f, 0f), 0.5f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void SwitchOffMonitor()
    {
        _monitorScreen.SetActive(false);
        IsMonitorOn = false;
        _deskCamera.transform.DOLocalMove(_deskCamera.transform.localPosition + new Vector3(0, -0.4f, 2.23f), 0.5f);
        _deskCamera.transform.DOLocalRotate(_deskCamera.transform.localEulerAngles + new Vector3(-8.512f, 0f, 0f), 0.5f);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
