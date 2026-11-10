using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    [SerializeField] private ItemDatabaseSO allItems;
    public LevelSO[] AllLevels;
    public int CurrentLevel;
    public Transform bagRoot;
    public GameObject CellPrefab;
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    void Start()
    {
        LevelSO currentLevel = AllLevels[CurrentLevel];
        for(int i = 0; i < currentLevel.itemsToSpawnIDs.Count; i++)
        {
            ItemDefinition currentItemData = allItems.GetItemByID(currentLevel.itemsToSpawnIDs[i]);
            GameObject currentGameObject = new GameObject(currentItemData.itemName);
            ItemDragHandler currentItem = currentGameObject.AddComponent<ItemDragHandler>();
            currentItem.cellPrefab = CellPrefab;
            currentItem.itemData = currentItemData;
            currentItem.bagRoot = bagRoot;
            currentItem.Init(); // nguyên nội dung hàm start
        }

    }
    public void CheckWin()
    {
        
    }
}
