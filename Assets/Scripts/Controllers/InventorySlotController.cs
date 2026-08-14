using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controls a single inventory slot, handling visual updates and stored powerup data
public class InventorySlotController : MonoBehaviour
{
    // Slot is considered empty when no PowerupData is assigned
    public bool IsEmpty => PowerupData == null;

    // Shortcut to the powerup's typable name
    public string Name => PowerupData.TypableName;

    // Powerup currently stored in this slot
    public PowerupData PowerupData;

    // UI elements for icon, name, and name container background
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private GameObject _nameContainer;

    // Assigns a powerup to this slot and updates UI
    public void SetItem(PowerupData powerupData)
    {
        PowerupData = powerupData;

        // Display icon and name
        _icon.sprite = powerupData.Icon;
        _nameText.text = powerupData.TypableName;
        _icon.color = Color.white;

        // Autocorrect powerups hide the displayed name
        if (powerupData.Type == PowerupType.Autocorrect)
        {
            _nameContainer.SetActive(false);
        }
        else
        {
            _nameContainer.SetActive(true);
        }
    }

    // Clears the slot and resets visuals
    public void EmptySlot()
    {
        PowerupData = null;

        // Remove icon and name
        _icon.sprite = null;
        _nameText.text = "";

        // Apply faded icon color to indicate empty state
        _icon.color = new Color32(224, 248, 207, 255);

        // Hide name container since no item is present
        _nameContainer.SetActive(false);
    }

    // Highlights part of the powerup name based on the typed prefix
    public void HighlightPrefix(int length)
    {
        string word = PowerupData.TypableName;
        if (string.IsNullOrEmpty(word))
            return;

        // Clamp highlight length to valid bounds
        length = Mathf.Clamp(length, 0, word.Length);

        string prefix = word.Substring(0, length);
        string suffix = word.Substring(length);

        // Color prefix green to show correct typed portion
        string coloredPrefix = $"<color=#86C06C>{prefix}</color>";

        _nameText.text = coloredPrefix + suffix;
    }

    // Resets name text after highlighting
    public void ResetHighlight()
    {
        if (PowerupData == null)
            return;

        // Restore original name
        _nameText.text = PowerupData.TypableName;
    }
}
