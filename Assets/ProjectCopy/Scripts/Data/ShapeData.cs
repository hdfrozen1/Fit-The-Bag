using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "Scriptable Objects/ShapeData")]
public class ShapeData : ScriptableObject
{
    [Tooltip("Số hàng của ma trận hình dạng")]
    public int Rows = 1;

    [Tooltip("Số cột của ma trận hình dạng")]
    public int Cols = 1;

    [Tooltip("hình dạng của vật \n độ dài chuỗi = Cols \n số phần tử = Rows")]
    public string[] HinhDang;
    //vi du hinh dang
    // dong 1 : 11
    // dong 2 : 10

}

[System.Serializable]
public class ItemDefinition
{
    [Header("Thông tin cơ bản")]
    public string itemID;           // VD: "chia_khoa"
    public string itemName;         // VD: "Chìa khóa vàng"
    public Sprite artwork;          // Hình ảnh hiển thị

    public ShapeData shapeData;             
    

    [Header("Cơ chế Hợp nhất (Merge)")]
    // Nếu món đồ này có thể hợp nhất với món khác, điền ID vào đây
    public string mergeTargetID;    // VD: "o_khoa"
    public string resultItemID;     // VD: "khoa_da_mo" (ID của món đồ mới sau khi ghép thành công)

}