using System.Collections;
using TMPro;
using UnityEngine;

public class DesktopController : Singleton<DesktopController>
{
    [SerializeField] private GameObject _window;
    [SerializeField] private GameObject _emailAppUI;
    [SerializeField] private GameObject _browserUI;
    [SerializeField] private TextMeshProUGUI _dateTimeText;

    private void OnEnable()
    {
        StartCoroutine(UpdateDateTime());
    }

    private void OnDisable()
    {
        StopCoroutine(UpdateDateTime());
    }

    private IEnumerator UpdateDateTime()
    {
        while (true)
        {
            _dateTimeText.text = System.DateTime.Now.AddYears(20).ToString("hh:mm tt\ndd/MM/yyyy");
            yield return new WaitForSeconds(1f);
        }
    }

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
        BrowserController.Instance.OpenSearchPage();
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
