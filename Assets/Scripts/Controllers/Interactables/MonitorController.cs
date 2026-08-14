using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class MonitorController : MonoBehaviour, IInteractable
{
    public bool IsMonitorOn = false;
    [SerializeField] private GameObject _monitorScreen;
    [SerializeField] private CinemachineCamera _deskCamera;
    [SerializeField] private GameObject _interactionTrigger;
    [SerializeField] private RenderTexture computerRenderTexture;

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

    public void ShowInteractionTrigger()
    {
        _interactionTrigger.SetActive(true);
    }

    public void HideInteractionTrigger()
    {
        _interactionTrigger.SetActive(false);
    }

    private void SwitchOnMonitor()
    {
        HideInteractionTrigger();

        _monitorScreen.SetActive(true);
        IsMonitorOn = true;
        _deskCamera.transform.DOLocalMove(_deskCamera.transform.localPosition + new Vector3(0, 0.43f, -2.05f), 0.5f);
        _deskCamera.transform.DOLocalRotate(_deskCamera.transform.localEulerAngles + new Vector3(8.512f, 0f, 0f), 0.5f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void SwitchOffMonitor()
    {
        _monitorScreen.SetActive(false);
        IsMonitorOn = false;
        _deskCamera.transform.DOLocalMove(_deskCamera.transform.localPosition + new Vector3(0, -0.43f, 2.05f), 0.5f);
        _deskCamera.transform.DOLocalRotate(_deskCamera.transform.localEulerAngles + new Vector3(-8.512f, 0f, 0f), 0.5f);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
