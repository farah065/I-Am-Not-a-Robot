using UnityEngine;

public class LampController : MonoBehaviour, IInteractable
{
    [SerializeField] private Light _pointLight;
    [SerializeField] private Light _ceilingSpotlight;
    [SerializeField] private Light _floorSpotlight;
    [SerializeField] private GameObject _interactionTrigger;

    private bool _isOn = false;

    private void Start()
    {
        SwitchOffLights();
    }

    private void SwitchOnLights()
    {
        _pointLight.enabled = true;
        _ceilingSpotlight.intensity = 60;
        _floorSpotlight.intensity = 100;
    }

    private void SwitchOffLights()
    {
        _pointLight.enabled = false;
        _ceilingSpotlight.intensity = 0;
        _floorSpotlight.intensity = 20;
    }

    public void Interact()
    {
        if (_isOn)
        {
            MusicController.Instance.PlayLightSwitchOffSfx();
            SwitchOffLights();
            _isOn = false;
        }
        else
        {
            MusicController.Instance.PlayLightSwitchOnSfx();
            SwitchOnLights();
            _isOn = true;
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
}
