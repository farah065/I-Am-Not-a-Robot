using UnityEngine;

public class EmailController : MonoBehaviour
{
    [SerializeField] private GameObject _landingUI;
    [SerializeField] private GameObject _emailUI;

    public void OpenEmail()
    {
        _landingUI.SetActive(false);
        _emailUI.SetActive(true);
    }

    public void CloseEmail()
    {
        _landingUI.SetActive(true);
        _emailUI.SetActive(false);
    }

    private void OnEnable()
    {
        CloseEmail();
    }
}
