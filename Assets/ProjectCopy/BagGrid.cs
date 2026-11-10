using System.Collections.Generic;
using UnityEngine;

public class BagGrid : MonoBehaviour
{
    [Header("Bag Shape Data")]
    public ShapeData BagShape;
    private int[,] bagData;
    public int Rows = 5;       // Số hàng
    public int Cols = 4;       // Số cột
    public float cellSize = 1f; // Kích thước mỗi ô vuông
    public float distanceBetweenCell = 0;

    public GameObject cellPrefab; // Prefab của ô vuông (Sprite hình vuông đơn giản)
    private Cell[,] cells;
    private List<Cell> cellsForAvaiable = new List<Cell>();
    void Start()
    {
        GenerateBagShapeAndData();
        GenerateGrid();
    }
    public void GenerateBagShapeAndData()
    {
        if (BagShape != null)
        {
            Rows = BagShape.Rows;
            Cols = BagShape.Cols;
            bagData = new int[Rows, Cols];
            cells = new Cell[Rows, Cols];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    bagData[i, j] = BagShape.HinhDang.Length != 0 ? BagShape.HinhDang[i][j] - '0' : 0;

                }
            }
        }
    }

    void GenerateGrid()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (bagData[r, c] == 0)
                {
                    // Tính tọa độ local. Chú ý: Trục Y đi xuống (-) để khớp với hàng r
                    Vector2 localPos = new Vector2(c * cellSize + c * distanceBetweenCell, -r * cellSize - r * distanceBetweenCell);

                    // Sinh ra ô vuông và đặt nó làm con của Balo (Object Cha)
                    GameObject cell = Instantiate(cellPrefab, transform);
                    cell.transform.localPosition = localPos;
                    cell.name = $"BagCell_{r}_{c}";

                    cells[r, c] = cell.GetComponent<Cell>();
                }

            }
        }
    }
    public void ShowGreenCell(Vector2 anchor, int rows, int cols, int[,] data, int greenCellCount)
    {
        if (cellsForAvaiable.Count != 0)
        {
            foreach (Cell item in cellsForAvaiable)
            {
                item.ChangeColorToNormal();
            }
            cellsForAvaiable.Clear();
        }
        if ((int)anchor.y + rows > this.Rows || (int)anchor.y < 0 || (int)anchor.x + cols > this.Cols || (int)anchor.x < 0)
        {
            return;
        }
        for (int i = (int)anchor.y; i < (int)anchor.y + rows; i++)
        {
            for (int j = (int)anchor.x; j < (int)anchor.x + cols; j++)
            {
                if (bagData[i, j] == 0 && data[i - (int)anchor.y, j - (int)anchor.x] == 1)
                {
                    cellsForAvaiable.Add(cells[i, j]);

                }
            }
        }
        if (cellsForAvaiable.Count == greenCellCount)
        {
            foreach (Cell item in cellsForAvaiable)
            {
                item.ChangeColorToAvaiable();
            }
        }
    }
    public bool CheckCollision(Vector2 anchor, int rows, int cols, int[,] data)
    {
        // (int)anchor.x, (int)anchor.y là VỊ TRÍ của ô => [(int)anchor.y,(int)anchor.x] là dữ liệu ô đó trên bagData
        bool result = true;
        Debug.Log("(int)anchor.y + rows: " + (int)anchor.y + " " + rows);
        Debug.Log("(int)anchor.x + cols: " + (int)anchor.x + " " + cols);
        Debug.Log("this Rows :" + this.Rows);
        Debug.Log("this Cols :" + this.Cols);
        if ((int)anchor.y + rows > this.Rows || (int)anchor.y < 0 || (int)anchor.x + cols > this.Cols || (int)anchor.x < 0)
        {
            return false;
        }
        for (int i = (int)anchor.y; i < (int)anchor.y + rows; i++)
        {
            for (int j = (int)anchor.x; j < (int)anchor.x + cols; j++)
            {
                if (bagData[i, j] == 1 && data[i - (int)anchor.y, j - (int)anchor.x] == 1)
                {
                    result = false;
                    break;
                }
            }
        }
        return result;
    }
    public void ChangeBagGridData(Vector2 anchor, int rows, int cols, int[,] data, bool isInTheBag)
    {
        for (int i = (int)anchor.y; i < (int)anchor.y + rows; i++)
        {
            for (int j = (int)anchor.x; j < (int)anchor.x + cols; j++)
            {
                if (!isInTheBag)
                {
                    if (bagData[i, j] == 0 && data[i - (int)anchor.y, j - (int)anchor.x] == 1)
                    {
                        bagData[i, j] = 1;
                    }
                }
                else
                {
                    if (bagData[i, j] == 1 && data[i - (int)anchor.y, j - (int)anchor.x] == 1)
                    {
                        bagData[i, j] = 0;
                    }
                }

            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GenerateGrid();
        }
    }
}