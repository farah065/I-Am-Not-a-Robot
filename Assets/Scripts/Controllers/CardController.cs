using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private Image _image;
    [SerializeField] private PowerupData _powerupData;

    public void SetPowerupData(PowerupData powerupData)
    {
        _powerupData = powerupData;

        _nameText.text = _powerupData.TypableName;
        _descriptionText.text = _powerupData.Description;
        _costText.text = _powerupData.Cost.ToString();
        _image.sprite = _powerupData.Icon;
    }
}
