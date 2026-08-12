using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class Settings : Singleton<Settings>
{
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _backToDesktopButton;
    [SerializeField] private VirtualScreen _virtualScreen;
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private GameObject _game;

    private void OnEnable()
    {
        _inputActions.FindActionMap("UI").FindAction("Cancel").performed += ToggleSettings;
    }

    private void OnDisable()
    {
        _inputActions.FindActionMap("UI").FindAction("Cancel").performed -= ToggleSettings;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetMasterVolume(float volume)
    {
        float logVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        _audioMixer.SetFloat("MasterVolume", logVolume);
    }

    public void SetMusicVolume(float volume)
    {
        float logVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        _audioMixer.SetFloat("MusicVolume", logVolume);
    }

    public void SetSFXVolume(float volume)
    {
        float logVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        _audioMixer.SetFloat("SfxVolume", logVolume);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void BackToDesktop()
    {
        HideSettings();
        _game.SetActive(false);
        DesktopController.Instance.CloseWindow();
    }

    public void ShowSettings()
    {
        Time.timeScale = 0f;
        _backToDesktopButton.SetActive(_game.activeSelf);
        _settingsMenu.SetActive(true);
        _virtualScreen.SetScreenCaster(ScreenCaster.Settings);
    }

    public void HideSettings()
    {
        Time.timeScale = 1f;
        _settingsMenu.SetActive(false);
        _virtualScreen.SetScreenCaster(ScreenCaster.Desktop);
    }

    private void ToggleSettings(InputAction.CallbackContext context)
    {
        if (_settingsMenu.activeSelf)
        {
            HideSettings();
        }
        else
        {
            ShowSettings();
        }
    }
}
