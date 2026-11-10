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
    [Header("Cấu hình Lưới")]
    public float cellSize = 1f;
    public float distanceBetweenCell = 0;
    public Transform bagRoot;
    [Header("Trạng thái Scale")]
    public float smallScale = 0.5f; // Kích thước khi nằm ngoài túi (thu nhỏ)
    public float normalScale = 1f;  // Kích thước khi cầm lên bằng đúng ô lưới
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
            // khoi tao trang thai scale va cell con
            currentItem.cellPrefab = CellPrefab;
            currentItem.smallScale = smallScale;
            currentItem.normalScale = normalScale;
            // khoi tao du lieu
            currentItem.itemData = currentItemData;
            currentItem.bagRoot = bagRoot;
            currentItem.cellSize = cellSize;
            currentItem.distanceBetweenCell =distanceBetweenCell;
            currentItem.Init(); // nguyên nội dung hàm start
        }

    }
    public void CheckWin()
    {
        
    }
}
