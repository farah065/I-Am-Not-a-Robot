using UnityEngine;

public class LampController : MonoBehaviour, IInteractable
{
    [SerializeField] private Light _pointLight;
    [SerializeField] private Light _ceilingSpotlight;
    [SerializeField] private Light _floorSpotlight;

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
            SwitchOffLights();
            _isOn = false;
        }
        else
        {
            SwitchOnLights();
            _isOn = true;
        }
    }
}
