using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems; // Cần thiết để dùng các interface kéo thả

/*public class ItemDragHandler : MonoBehaviour
{
    private Vector3 rootPos;
    private int greenCellCount = 0;
    private Vector2 anchorPointInBag;
    private bool isInTheBag = false;
    public bool IsInTheBag => isInTheBag;
    [Header("dữ liệu hình")]
    public ShapeData ShapeData;
    public int Row;
    public int Col;
    [Header("Cấu hình Lưới")]
    public float cellSize = 1f;
    public float distanceBetweenCell = 0;
    public Transform bagRoot; // Kéo object Balo vào đây để tính mốc tọa độ

    [Header("Trạng thái Scale")]
    public float smallScale = 0.5f; // Kích thước khi nằm ngoài túi (thu nhỏ)
    public float normalScale = 1f;  // Kích thước khi cầm lên bằng đúng ô lưới

    public GameObject cellPrefab;   // Prefab ô vuông giống hệt Balo

    // Ma trận hình dáng (Ví dụ: Hình chữ L)
    // 1 là có khối thịt, 0 là khoảng trống
    private GameObject collisionGameObject;
    private int[,] shape = new int[,]
    {
        { 1, 0 },
        { 1, 0 },
        { 1, 1 }
    };

    private Vector3 dragOffset; // Lưu độ lệch giữa ngón tay và gốc Object

    void Start()
    {
        rootPos = transform.position;
        GenerateShape();
        GenerateItemCells();

        // Khởi tạo vật phẩm ở trạng thái thu nhỏ
        transform.localScale = new Vector3(smallScale, smallScale, 1f);
    }
    void GenerateShape()
    {
        if (shape != null)
        {

            Row = ShapeData.Rows;
            Col = ShapeData.Cols;
            shape = new int[Row, Col];

            for (int i = 0; i < Row; i++)
            {

                for (int j = 0; j < Col; j++)
                {
                    shape[i, j] = ShapeData.HinhDang[i][j] - '0';
                }

            }
        }
    }

    void GenerateItemCells()
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (shape[r, c] == 1) // Chỉ sinh ô vuông ở chỗ có giá trị 1
                {
                    Vector2 localPos = new Vector2(c * cellSize + c * distanceBetweenCell, -r * cellSize - r * distanceBetweenCell);
                    GameObject cell = Instantiate(cellPrefab, transform);
                    cell.transform.localPosition = localPos;
                    cell.name = $"ItemCell_{r}_{c}";
                    greenCellCount +=1;
                }
            }
        }
    }
    /// <summary>
    /// xem vị trí gốc của object ứng với ô nào trên board.
    /// Trả về 2 số nguyên x, y.Cột thứ x, hàng thứ y.
    /// Tương ứng với bagData[y,x]
    /// </summary>
    /// <returns> Vector2(int x,int y) </returns>
    Vector2 GetAnchorPointInBag()
    {
        // Lấy tọa độ thế giới của Object Balo và Vật phẩm hiện tại
        float bagX = bagRoot.position.x;
        float bagY = bagRoot.position.y;

        float itemX = transform.position.x;
        float itemY = transform.position.y;

        // Tính ra AnchorX (Cột) và AnchorY (Hàng) bằng toán làm tròn
        // (Chú ý: Vì Y đi xuống nên tính theo chiều ngược: bagY - itemY)
        int anchorX = Mathf.RoundToInt((itemX - bagX) / (cellSize + distanceBetweenCell));
        int anchorY = Mathf.RoundToInt((bagY - itemY) / (cellSize + distanceBetweenCell));
        return new Vector2(anchorX,anchorY);
    }

    // ----------------- CƠ CHẾ KÉO THẢ -----------------

    public void HandlePointerDown(PointerEventData eventData)
    {
        if (isInTheBag)
        {
            bagRoot.GetComponent<BagGrid>().ChangeBagGridData(anchorPointInBag,Row,Col,shape,isInTheBag);
            isInTheBag = false;
        }
        // 1. Phóng to vật phẩm bằng với kích thước lưới
        transform.localScale = new Vector3(normalScale, normalScale, 1f);

        // 2. Tính toán khoảng cách chênh lệch (Offset) giữa điểm chạm và Gốc Object
        // Giúp vật phẩm không bị nhảy giật tâm về giữa ngón tay khi vừa bấm vào
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        dragOffset = transform.position - mousePos;
    }

    public void HandleDrag(PointerEventData eventData)
    {
        // 3. Di chuyển vật phẩm đi theo ngón tay (cộng thêm offset)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos + dragOffset;

        anchorPointInBag = GetAnchorPointInBag();
        bagRoot.GetComponent<BagGrid>().ShowGreenCell(anchorPointInBag,Row,Col,shape,greenCellCount);
    }

    public void HandlePointerUp(PointerEventData eventData)
    {
        // 4. THUẬT TOÁN SNAP (HÚT VÀO LƯỚI)    

        anchorPointInBag = GetAnchorPointInBag();
        bool notCollision = bagRoot.GetComponent<BagGrid>().CheckCollision(anchorPointInBag,Row,Col,shape);
        if (!notCollision)
        {
            if (!collisionGameObject.GetComponent<ItemDragHandler>().IsInTheBag)
            {
                //hợp nhất, nếu không thì trả về vị trí cũ
            }
            transform.position = rootPos;
            isInTheBag = false;
            return;
        }

        // Tạm thời bỏ qua Rules và Va chạm, ta ép vị trí Vật phẩm khớp chặt vào ô neo vừa tính
        float snappedX = bagRoot.position.x + ((int)anchorPointInBag.x * cellSize) + ((int)anchorPointInBag.x) * distanceBetweenCell;
        float snappedY = bagRoot.position.y - ((int)anchorPointInBag.y * cellSize) - ((int)anchorPointInBag.y) * distanceBetweenCell;

        transform.position = new Vector3(snappedX, snappedY, 0);
        bagRoot.GetComponent<BagGrid>().ChangeBagGridData(anchorPointInBag,Row,Col,shape,isInTheBag);
        isInTheBag = true; 

        // Tùy chọn: Nếu bạn muốn nó thu nhỏ lại khi thả ra ngoài (chưa làm ở bước này)
        // transform.localScale = new Vector3(smallScale, smallScale, 1f);
    }
    public void RegisterCollision(GameObject gameObject)
    {
        collisionGameObject = gameObject;
    }
    public void UnregisterCollision(GameObject gameObject)
    {
        collisionGameObject = null;
    }

}*/
public class ItemDragHandler : MonoBehaviour
{
    private Vector3 rootPos;
    private int greenCellCount = 0;
    private Vector2 anchorPointInBag;
    private bool isInTheBag = false;
    public bool IsInTheBag => isInTheBag;
    [Header("dữ liệu hình")]
    public ItemDefinition itemData;
    private int Row;
    private int Col;
    [Header("Cấu hình Lưới")]
    public float cellSize = 1f;
    public float distanceBetweenCell = 0;
    public Transform bagRoot; // Kéo object Balo vào đây để tính mốc tọa độ

    [Header("Trạng thái Scale")]
    public float smallScale = 0.5f; // Kích thước khi nằm ngoài túi (thu nhỏ)
    public float normalScale = 1f;  // Kích thước khi cầm lên bằng đúng ô lưới

    public GameObject cellPrefab;   // Prefab ô vuông giống hệt Balo

    // Ma trận hình dáng (Ví dụ: Hình chữ L)
    // 1 là có khối thịt, 0 là khoảng trống
    private GameObject collisionGameObject;
    private int[,] shape = new int[,]
    {
        { 1, 0 },
        { 1, 0 },
        { 1, 1 }
    };

    private Vector3 dragOffset; // Lưu độ lệch giữa ngón tay và gốc Object

    public void Init()
    {
        rootPos = transform.position;
        GenerateShape();
        GenerateItemCells();
        GenerateSprite();
        // Khởi tạo vật phẩm ở trạng thái thu nhỏ
        transform.localScale = new Vector3(smallScale, smallScale, 1f);
    }
    void GenerateShape()
    {
        if (shape != null)
        {

            Row = itemData.shapeData.Rows;
            Col = itemData.shapeData.Cols;
            shape = new int[Row, Col];

            for (int i = 0; i < Row; i++)
            {

                for (int j = 0; j < Col; j++)
                {
                    shape[i, j] = itemData.shapeData.HinhDang[i][j] - '0';
                }

            }
        }
    }

    void GenerateItemCells()
    {
        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (shape[r, c] == 1) // Chỉ sinh ô vuông ở chỗ có giá trị 1
                {
                    Vector2 localPos = new Vector2(c * cellSize + c * distanceBetweenCell, -r * cellSize - r * distanceBetweenCell);
                    GameObject cell = Instantiate(cellPrefab, transform);
                    cell.transform.localPosition = localPos;
                    cell.name = $"ItemCell_{r}_{c}";
                    greenCellCount += 1;
                }
            }
        }
    }
    void GenerateSprite()
    {
        Vector2 firstPos = new Vector2(0 * cellSize + 0 * distanceBetweenCell, -0 * cellSize - 0 * distanceBetweenCell);
        int lastRowCell = Row - 1;
        int lastColCell = Col - 1;
        Vector2 lastPos = new Vector2(lastRowCell * cellSize + lastRowCell * distanceBetweenCell, -lastColCell * cellSize - lastColCell * distanceBetweenCell);
        Vector2 spritePos = (lastPos + firstPos) / 2;
        if (itemData.artwork != null)
        {
            GameObject sprite = new GameObject();
            sprite.transform.parent = this.transform;
            SpriteRenderer currentSprite = sprite.AddComponent<SpriteRenderer>();
            currentSprite.sprite = itemData.artwork;
            sprite.transform.localPosition = new Vector3(spritePos.x,spritePos.y,-1);
        }

    }
    /// <summary>
    /// xem vị trí gốc của object ứng với ô nào trên board.
    /// Trả về 2 số nguyên x, y.Cột thứ x, hàng thứ y.
    /// Tương ứng với bagData[y,x]
    /// </summary>
    /// <returns> Vector2(int x,int y) </returns>
    Vector2 GetAnchorPointInBag()
    {
        // Lấy tọa độ thế giới của Object Balo và Vật phẩm hiện tại
        float bagX = bagRoot.position.x;
        float bagY = bagRoot.position.y;

        float itemX = transform.position.x;
        float itemY = transform.position.y;

        // Tính ra AnchorX (Cột) và AnchorY (Hàng) bằng toán làm tròn
        // (Chú ý: Vì Y đi xuống nên tính theo chiều ngược: bagY - itemY)
        int anchorX = Mathf.RoundToInt((itemX - bagX) / (cellSize + distanceBetweenCell));
        int anchorY = Mathf.RoundToInt((bagY - itemY) / (cellSize + distanceBetweenCell));
        return new Vector2(anchorX, anchorY);
    }

    // ----------------- CƠ CHẾ KÉO THẢ -----------------

    public void HandlePointerDown(PointerEventData eventData)
    {
        if (isInTheBag)
        {
            bagRoot.GetComponent<BagGrid>().ChangeBagGridData(anchorPointInBag, Row, Col, shape, isInTheBag);
            isInTheBag = false;
        }
        // 1. Phóng to vật phẩm bằng với kích thước lưới
        transform.localScale = new Vector3(normalScale, normalScale, 1f);

        // 2. Tính toán khoảng cách chênh lệch (Offset) giữa điểm chạm và Gốc Object
        // Giúp vật phẩm không bị nhảy giật tâm về giữa ngón tay khi vừa bấm vào
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        dragOffset = transform.position - mousePos;
    }

    public void HandleDrag(PointerEventData eventData)
    {
        // 3. Di chuyển vật phẩm đi theo ngón tay (cộng thêm offset)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos + dragOffset;

        anchorPointInBag = GetAnchorPointInBag();
        bagRoot.GetComponent<BagGrid>().ShowGreenCell(anchorPointInBag, Row, Col, shape, greenCellCount);
    }

    public void HandlePointerUp(PointerEventData eventData)
    {
        // 4. THUẬT TOÁN SNAP (HÚT VÀO LƯỚI)    

        anchorPointInBag = GetAnchorPointInBag();
        bool notCollision = bagRoot.GetComponent<BagGrid>().CheckCollision(anchorPointInBag, Row, Col, shape);
        if (!notCollision)
        {
            // if (!collisionGameObject && !collisionGameObject.GetComponent<ItemDragHandler>().IsInTheBag)
            // {
            //     //hợp nhất, nếu không thì trả về vị trí cũ
            // }
            transform.position = rootPos;
            isInTheBag = false;
            return;
        }

        // Tạm thời bỏ qua Rules và Va chạm, ta ép vị trí Vật phẩm khớp chặt vào ô neo vừa tính
        float snappedX = bagRoot.position.x + ((int)anchorPointInBag.x * cellSize) + ((int)anchorPointInBag.x) * distanceBetweenCell;
        float snappedY = bagRoot.position.y - ((int)anchorPointInBag.y * cellSize) - ((int)anchorPointInBag.y) * distanceBetweenCell;

        transform.position = new Vector3(snappedX, snappedY, 0);
        bagRoot.GetComponent<BagGrid>().ChangeBagGridData(anchorPointInBag, Row, Col, shape, isInTheBag);
        isInTheBag = true;

        // Tùy chọn: Nếu bạn muốn nó thu nhỏ lại khi thả ra ngoài (chưa làm ở bước này)
        // transform.localScale = new Vector3(smallScale, smallScale, 1f);
    }
    public void RegisterCollision(GameObject gameObject)
    {
        collisionGameObject = gameObject;
    }
    public void UnregisterCollision(GameObject gameObject)
    {
        collisionGameObject = null;
    }

}