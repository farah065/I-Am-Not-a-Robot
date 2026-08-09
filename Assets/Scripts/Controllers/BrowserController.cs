using TMPro;
using UnityEngine;

public class BrowserController : Singleton<BrowserController>
{
    [SerializeField] private GameObject _searchPage;
    [SerializeField] private GameObject _valdivian;
    [SerializeField] private GameObject _login;
    [SerializeField] private GameObject _captcha;

    [SerializeField] private TMP_InputField _email;
    [SerializeField] private TMP_InputField _password;

    [SerializeField] private GameObject _game;

    public void OpenSearchPage()
    {
        _searchPage.SetActive(true);
        _valdivian.SetActive(false);
    }
    
    public void TryOpenValdivianPage(string query)
    {
        if (query.ToLower().Equals("valdivian.com"))
        {
            OpenValdivianPage();
        }
    }

    public void TryLogIn()
    {
        if (_email.text.ToLower().Equals("robbie@valdivian.com") && _password.text.Equals("password"))
        {
            ShowCaptcha();
        }
    }

    public void ShowCaptcha()
    {
        _login.SetActive(false);
        _captcha.SetActive(true);
    }

    public void ShowGame()
    {
        _game.SetActive(true);
        _captcha.SetActive(false);
    }

    public void HideGame()
    {
        _captcha.SetActive(true);
        _game.SetActive(false);
    }

    private void OpenValdivianPage()
    {
        _searchPage.SetActive(false);
        _valdivian.SetActive(true);
        _login.SetActive(true);
        _captcha.SetActive(false);
    }

    private void OnEnable()
    {
        OpenSearchPage();
    }
}
