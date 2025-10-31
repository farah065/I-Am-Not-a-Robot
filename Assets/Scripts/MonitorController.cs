using UnityEngine;

public class MonitorController : MonoBehaviour, IInteractable
{
    public bool IsMonitorOn = false;
    [SerializeField] private GameObject _monitorScreen;

    public void Interact()
    {
        if (IsMonitorOn)
        {
            _monitorScreen.SetActive(false);
            IsMonitorOn = false;
        }
        else
        {
            _monitorScreen.SetActive(true);
            IsMonitorOn = true;
        }
    }
}
