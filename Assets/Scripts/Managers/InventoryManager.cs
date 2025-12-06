using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private InventorySlotController[] _inventorySlots;

    public void AddItemToInventory(PowerupData powerupData)
    {
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            if (_inventorySlots[i].IsEmpty)
            {
                _inventorySlots[i].SetItem(powerupData);
                break;
            }
        }
    }
}
