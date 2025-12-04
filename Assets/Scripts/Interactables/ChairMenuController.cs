using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ChairMenuController : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    private InputAction _navigateAction;
    private InputAction _submitAction;

    [SerializeField] private Button _pullOutButton;
    [SerializeField] private Button _pushInButton;
    [SerializeField] private Button _sitDownButton;
    [SerializeField] private ChairController _chairController;
    private List<Button> _activeButtons;
    private int _selectedButtonIndex = 0;

    private void Awake()
    {
        _navigateAction = _inputActions.FindActionMap("UI").FindAction("Navigate");
        _navigateAction.performed += CycleSelection;

        _submitAction = _inputActions.FindActionMap("UI").FindAction("Submit");
        _submitAction.performed += context =>
        {
            _activeButtons[_selectedButtonIndex].onClick.Invoke();
        };
    }

    private void OnEnable()
    {
        _navigateAction.Enable();
        _submitAction.Enable();

        if (_chairController.IsPushedIn)
        {
            _pullOutButton.gameObject.SetActive(true);
            _pushInButton.gameObject.SetActive(false);
            _sitDownButton.gameObject.SetActive(false);
            _activeButtons = new List<Button> { _pullOutButton };
        }
        else
        {
            _pullOutButton.gameObject.SetActive(false);
            _pushInButton.gameObject.SetActive(true);
            _sitDownButton.gameObject.SetActive(true);
            _activeButtons = new List<Button> { _pushInButton, _sitDownButton };
        }
        _selectedButtonIndex = 0;
        HighlightSelectedButton();
    }

    private void OnDisable()
    {
        _navigateAction.Disable();
        _submitAction.Disable();
    }

    private void HighlightSelectedButton()
    {
        foreach (Button button in _activeButtons)
        {
            if (button == _activeButtons[_selectedButtonIndex])
            {
                button.GetComponent<Image>().color = new Color(0.843f, 0.996f, 0.541f);
            }
            else
            {
                button.GetComponent<Image>().color = Color.white;
            }
        }
    }

    private void CycleSelection(InputAction.CallbackContext context)
    {
        Vector2 navigation = context.ReadValue<Vector2>();
        if (navigation.y > 0)
        {
            _selectedButtonIndex = (_selectedButtonIndex - 1 + _activeButtons.Count) % _activeButtons.Count;
        }
        else if (navigation.y < 0)
        {
            _selectedButtonIndex = (_selectedButtonIndex + 1) % _activeButtons.Count;
        }
        HighlightSelectedButton();
    }
}
