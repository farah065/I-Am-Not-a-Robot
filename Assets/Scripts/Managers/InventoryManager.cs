using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public Trie InventoryTrie;
    [SerializeField] private InventorySlotController[] _inventorySlots;

    private void Start()
    {
        InventoryTrie = new Trie();
    }

    public void AddItemToInventory(PowerupData powerupData)
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (_inventorySlots[i].IsEmpty)
            {
                _inventorySlots[i].SetItem(powerupData);
                InventoryTrie.Insert(powerupData.TypableName);
                break;
            }
        }
    }

    public bool IsInventoryFull()
    {
        foreach (var slot in _inventorySlots)
        {
            if (slot.IsEmpty)
                return false;
        }
        return true;
    }

    public PowerupData FindInventoryPowerup(string name)
    {
        foreach (var slot in GetComponentsInChildren<InventorySlotController>())
        {
            if (!slot.IsEmpty && slot.Name == name)
                return slot.PowerupData;
        }
        return null;
    }

    public void ApplyInventoryPowerup(PowerupData powerup)
    {
        Debug.Log("Used powerup: " + powerup.TypableName);
    }
}
