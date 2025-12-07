using UnityEngine;

// Manages the player's inventory and provides global access via Singleton pattern
public class InventoryManager : Singleton<InventoryManager>
{
    // Trie used for fast lookup of powerup names the player currently holds
    public Trie InventoryTrie;

    // Array of inventory slot controllers representing the inventory UI slots
    public InventorySlotController[] InventorySlots;

    private void Start()
    {
        // Initialize the Trie when the inventory manager starts
        InventoryTrie = new Trie();
    }

    // Attempts to add a powerup to the first available empty inventory slot
    public void AddItemToInventory(PowerupData powerupData)
    {
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            // Check for the first empty slot
            if (InventorySlots[i].IsEmpty)
            {
                // Place the powerup into the slot
                InventorySlots[i].SetItem(powerupData);

                // Only insert into Trie if not an Autocorrect powerup
                if (powerupData.Type != PowerupType.Autocorrect)
                {
                    InventoryTrie.Insert(powerupData.TypableName);
                }
                break;
            }
        }
    }

    // Returns true only if all inventory slots are filled
    public bool IsInventoryFull()
    {
        foreach (var slot in InventorySlots)
        {
            // If ANY slot is empty, the inventory is not full
            if (slot.IsEmpty)
                return false;
        }
        return true;
    }

    // Searches for a powerup in the inventory by its name
    public PowerupData FindInventoryPowerup(string name)
    {
        foreach (var slot in InventorySlots)
        {
            // Return the powerup if the slot is filled and the name matches
            if (!slot.IsEmpty && slot.Name == name)
                return slot.PowerupData;
        }
        return null; // Not found
    }

    // Uses a given powerup and removes it from the inventory
    public void UsePowerup(PowerupData powerup)
    {
        // Apply powerup effects based on type
        if (powerup.Type == PowerupType.Bandage)
        {
            Player2D.Instance.Heal(); // Heal the player
        }
        if (powerup.Type == PowerupType.Freeze)
        {
            Player2D.Instance.CanFreeze = true; // Allow freezing effect
        }

        // Remove the powerup's name from the Trie lookup structure
        InventoryTrie.Delete(powerup.TypableName);

        // Remove the item from its slot
        foreach (var slot in InventorySlots)
        {
            if (!slot.IsEmpty && slot.Name == powerup.TypableName)
            {
                slot.EmptySlot(); // Clear the slot
            }
        }
    }

    // Clears all inventory slots (removes all items)
    public void ClearInventory()
    {
        foreach (var slot in InventorySlots)
        {
            slot.EmptySlot(); // Clear each slot
        }
    }
}
