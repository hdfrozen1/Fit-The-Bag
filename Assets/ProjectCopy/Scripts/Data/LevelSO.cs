using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "Scriptable Objects/LevelSO")]
public class LevelSO : ScriptableObject
{
    [Tooltip("Chỉ cần điền ID của các món đồ muốn sinh ra ở màn này")]
    public List<string> itemsToSpawnIDs; 
    // Ví dụ list này chứa: ["chia_khoa", "o_khoa", "banh_mi"]
}
