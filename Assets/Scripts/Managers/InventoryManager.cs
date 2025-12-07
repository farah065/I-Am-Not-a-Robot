using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    public Trie InventoryTrie;
    public InventorySlotController[] InventorySlots;

    private void Start()
    {
        InventoryTrie = new Trie();
    }

    public void AddItemToInventory(PowerupData powerupData)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            if (InventorySlots[i].IsEmpty)
            {
                InventorySlots[i].SetItem(powerupData);
                InventoryTrie.Insert(powerupData.TypableName);
                break;
            }
        }
    }

    public bool IsInventoryFull()
    {
        foreach (var slot in InventorySlots)
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
        
        InventoryTrie.Delete(powerup.TypableName);

        foreach (var slot in InventorySlots)
        {
            if (!slot.IsEmpty && slot.Name == powerup.TypableName)
            {
                slot.EmptySlot();
            }
        }
    }
}
