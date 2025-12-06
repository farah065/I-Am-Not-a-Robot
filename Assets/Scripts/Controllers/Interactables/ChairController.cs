using UnityEngine;
using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;

public class ChairController : MonoBehaviour, IInteractable
{
    public bool IsPushedIn = true;
    public bool IsPlayerSitting = false;
    [SerializeField] private GameObject _menuCanvas;
    [SerializeField] private CinemachineCamera _deskCamera;
    private bool _isMenuOpen = false;

    public void Interact()
    {
        if (!IsPlayerSitting)
        {
            if (_isMenuOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    public void SitDown()
    {
        CloseMenu();
        IsPlayerSitting = true;
        _deskCamera.Priority = 2;
        Sequence sitSequence = DOTween.Sequence();
        sitSequence.Append(_deskCamera.transform.DOMove(_deskCamera.transform.position + new Vector3(0, -1f, 0), 0.5f))
            .PrependInterval(0.75f);
        sitSequence.Play();
    }

    public void StandUp()
    {
        IsPlayerSitting = false;
        _deskCamera.Priority = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(StandUpCoroutine());
    }

    private IEnumerator StandUpCoroutine()
    {
        _deskCamera.transform.DOMove(_deskCamera.transform.position + new Vector3(0, 1f, 0), 0.5f);
        yield return new WaitForSeconds(0.75f);
        _deskCamera.Priority = 0;
    }

    public void PullOut()
    {
        CloseMenu();
        DOTween.To(() => transform.position, x => transform.position = x, transform.position + new Vector3(0, 0, 2.25f), 0.5f);
        IsPushedIn = false;
    }

    public void PushIn()
    {
        CloseMenu();
        DOTween.To(() => transform.position, x => transform.position = x, transform.position - new Vector3(0, 0, 2.25f), 0.5f);
        IsPushedIn = true;
    }

    private void OpenMenu()
    {
        _menuCanvas.SetActive(true);
        _isMenuOpen = true;
    }

    private void CloseMenu()
    {
        _menuCanvas.SetActive(false);
        _isMenuOpen = false;
    }
}
