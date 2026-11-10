using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabaseSO", menuName = "Scriptable Objects/ItemDatabaseSO")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemDefinition> allItems; // Danh sách hàng trăm món đồ

    // Viết 1 hàm tìm kiếm nhanh
    public ItemDefinition GetItemByID(string id)
    {
        return allItems.Find(item => item.itemID == id);
    }
}
