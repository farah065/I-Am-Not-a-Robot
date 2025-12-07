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
                if (powerupData.Type != PowerupType.Autocorrect)
                {
                    InventoryTrie.Insert(powerupData.TypableName);
                }
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
        foreach (var slot in InventorySlots)
        {
            if (!slot.IsEmpty && slot.Name == name)
                return slot.PowerupData;
        }
        return null;
    }

    public void UsePowerup(PowerupData powerup)
    {
        if (powerup.Type == PowerupType.Bandage)
        {
            Player2D.Instance.Heal();
        }
        if (powerup.Type == PowerupType.Freeze)
        {
            Player2D.Instance.CanFreeze = true;
        }
        
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
