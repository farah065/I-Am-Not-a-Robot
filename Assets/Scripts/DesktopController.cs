using UnityEngine;

public class DesktopController : MonoBehaviour
{
    [SerializeField] private GameObject _emailAppUI;

    public void OpenEmailApp()
    {
        _emailAppUI.SetActive(true);
    }

    public void CloseEmailApp()
    {
        _emailAppUI.GetComponent<EmailController>().CloseEmail();
        _emailAppUI.SetActive(false);
    }
}
