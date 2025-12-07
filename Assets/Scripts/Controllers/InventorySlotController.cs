using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotController : MonoBehaviour
{
    public bool IsEmpty => _nameText.text.Length == 0;
    public string Name => _nameText.text;
    public PowerupData PowerupData;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;

    public void SetItem(PowerupData powerupData)
    {
        PowerupData = powerupData;
        _icon.sprite = powerupData.Icon;
        _nameText.text = powerupData.TypableName;
    }

    public void HighlightPrefix(int length)
    {
        string word = PowerupData.TypableName;
        if (string.IsNullOrEmpty(word))
            return;

        length = Mathf.Clamp(length, 0, word.Length);

        string prefix = word.Substring(0, length);
        string suffix = word.Substring(length);

        // Change this color to whatever highlight you want
        string coloredPrefix = $"<color=#86C06C>{prefix}</color>";

        _nameText.text = coloredPrefix + suffix;
    }

    public void ResetHighlight()
    {
        if (PowerupData == null)
            return;
        _nameText.text = PowerupData.TypableName;
    }
}
