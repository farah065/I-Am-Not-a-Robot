using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardController : MonoBehaviour
{
    public string NormalisedName;
    public PowerupData PowerupData;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private Image _image;

    public void SetPowerupData(PowerupData powerupData)
    {
        PowerupData = powerupData;
        NormalisedName = PowerupData.DisplayName.ToLower().Replace(" ", "");

        _nameText.text = PowerupData.DisplayName;
        _descriptionText.text = PowerupData.Description;
        _costText.text = PowerupData.Cost.ToString();
        _image.sprite = PowerupData.Icon;
    }

    public void HighlightPrefix(int length)
    {
        if (string.IsNullOrEmpty(PowerupData.DisplayName))
            return;

        int count = 0;
        string result = "";

        foreach (char c in PowerupData.DisplayName)
        {
            if (c != ' ' && count < length)
            {
                // This character contributes to the normalized prefix
                result += $"<color=#E0F8CF>{c}</color>";
                count++;
            }
            else
            {
                result += c;
            }
        }

        _nameText.text = result;
    }

    public void ResetHighlight()
    {
        _nameText.text = PowerupData.DisplayName;
    }
}
