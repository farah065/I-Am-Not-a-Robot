using UnityEngine;

public class DesktopController : Singleton<DesktopController>
{
    [SerializeField] private GameObject _window;
    [SerializeField] private GameObject _emailAppUI;
    [SerializeField] private GameObject _browserUI;

    public void CloseWindow()
    {
        _window.SetActive(false);
        _emailAppUI.SetActive(false);
        _browserUI.SetActive(false);
    }

    public void OpenEmailApp()
    {
        _window.SetActive(true);
        _emailAppUI.SetActive(true);
        _browserUI.SetActive(false);
    }

    public void CloseEmailApp()
    {
        _window.SetActive(false);
        _emailAppUI.SetActive(false);
    }

    public void OpenBrowser()
    {
        _window.SetActive(true);
        _browserUI.SetActive(true);
        _emailAppUI.SetActive(false);
    }

    public void CloseBrowser()
    {
        _window.SetActive(false);
        _browserUI.SetActive(false);
    }
}
