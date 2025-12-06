using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotController : MonoBehaviour
{
    public bool IsEmpty => _nameText.text.Length == 0;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _nameText;

    public void SetItem(PowerupData powerupData)
    {
        _icon.sprite = powerupData.Icon;
        _nameText.text = powerupData.TypableName;
    }
}
