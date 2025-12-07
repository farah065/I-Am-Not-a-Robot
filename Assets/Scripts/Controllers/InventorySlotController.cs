using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotController : MonoBehaviour
{
    public bool IsEmpty => PowerupData == null;
    public string Name => PowerupData.TypableName;
    public PowerupData PowerupData;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private GameObject _nameContainer;

    public void SetItem(PowerupData powerupData)
    {
        PowerupData = powerupData;
        _icon.sprite = powerupData.Icon;
        _nameText.text = powerupData.TypableName;
        _icon.color = Color.white;

        if (powerupData.Type == PowerupType.Autocorrect)
        {
            _nameContainer.SetActive(false);
        }
        else
        {
            _nameContainer.SetActive(true);
        }
    }

    public void EmptySlot()
    {
        PowerupData = null;
        _icon.sprite = null;
        _nameText.text = "";
        _icon.color = new Color32(224, 248, 207, 255);
        _nameContainer.SetActive(false);
    }

    public void HighlightPrefix(int length)
    {
        string word = PowerupData.TypableName;
        if (string.IsNullOrEmpty(word))
            return;

        length = Mathf.Clamp(length, 0, word.Length);

        string prefix = word.Substring(0, length);
        string suffix = word.Substring(length);

        string coloredPrefix = $"<color=#86C06C>{prefix}</color>";

        _nameText.text = coloredPrefix + suffix;
    }

    public void ResetHighlight()
    {
        if (PowerupData == null)
            return;

        Debug.Log("Resetting highlight for word: " + PowerupData.TypableName);
        _nameText.text = PowerupData.TypableName;
    }
}
