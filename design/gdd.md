# TECHNICAL GAME DESIGN DOCUMENT (GDD)
**Dự án:** Chiếc Balo Vừa Vặn (Fit The Backpack)  
**Nền tảng:** Mobile (Màn hình dọc - Touch Input)  
**Trọng tâm kỹ thuật:** Hệ thống Lưới (Grid Matrix), Kéo - Thả (Drag & Drop), và Kiểm tra Ràng buộc (Rule-based Validation).

---

## 1. TỔNG QUAN GAMEPLAY VÀ VÒNG LẶP CỐT LÕI (CORE LOOP)

### 1.1. Luồng trải nghiệm người chơi
1. **Bắt đầu ván (Setup):** Lưới balo hiển thị rỗng ở nửa trên màn hình; các vật phẩm cần xếp nằm ở khay chứa (Inventory Dock) ở nửa dưới.
2. **Thao tác (Interaction):**
   * Người chơi chạm và giữ vào vật phẩm trong khay để nhấc lên (PointerDown).
   * Kéo rê vật phẩm qua lưới balo (PointerDrag). Lưới hiển thị các ô bóng (Ghost/Preview) với màu sắc phản hồi (Xanh: Hợp lệ / Đỏ: Không hợp lệ).
   * Chạm nhẹ (Tap/Click nhanh) vào vật phẩm đang chọn để xoay $90^\circ$ theo chiều kim đồng hồ.
   * Thả tay (PointerUp): Nếu vị trí hợp lệ, vật phẩm gắn vào lưới (Snap). Nếu không, vật phẩm bay ngược về vị trí cũ trong khay chứa.
3. **Kiểm tra & Kết thúc (Validation & Win):**
   * Mỗi khi 100% vật phẩm nằm kín lưới, hoặc khi người chơi bấm nút "Kéo Khóa Balo", hệ thống kiểm tra toàn bộ các quy tắc ngầm (Rule System).
   * Nếu hợp lệ: Kích hoạt hiệu ứng khóa balo (Zipper Animation) và chuyển màn.
   * Nếu vi phạm: Đánh dấu đỏ các vật phẩm xung đột, rung nhẹ thiết bị (Haptic feedback) và hiển thị bong bóng cảm xúc giải thích lỗi.

---

## 2. HỆ THỐNG LƯỚI & MA TRẬN DỮ LIỆU (GRID SYSTEM)

### 2.1. Cấu trúc tọa độ
Lưới balo được biểu diễn bằng hệ tọa độ ma trận 2D $[rows, cols]$:
* Gốc tọa độ $(0, 0)$ nằm ở góc **trên cùng - bên trái** (Top-Left).
* Chiều $X$ tăng dần từ trái sang phải: $x \in [0, cols - 1]$.
* Chiều $Y$ tăng dần từ trên xuống dưới: $y \in [0, rows - 1]$.

### 2.2. Biểu diễn hình dạng vật phẩm (Shape Matrix)
Mỗi vật phẩm được định nghĩa bằng một ma trận nhị phân 2D:
* `1`: Ô vật lý bị chiếm dụng.
* `0`: Ô trống (dành cho các vật có hình thù đặc biệt như chữ L, chữ T).

**Ví dụ:**
```csharp
// Bình nước (1x3 dọc)
int[,] WaterBottleShape = new int[,] {
    { 1 },
    { 1 },
    { 1 }
};

// Hộp cơm chữ L (2x2)
int[,] LunchBoxShape = new int[,] {
    { 1, 0 },
    { 1, 1 }
};
```

### 2.3. Giải thuật Xoay Ma trận $90^\circ$ (Clockwise Matrix Rotation)
Khi xoay vật phẩm, ma trận hình dạng kích thước $M \times N$ sẽ biến đổi thành kích thước $N \times M$ theo công thức:
$$rotated[j, M - 1 - i] = original[i, j]$$

```
Input:               Output:
[1, 0]    ===>       [1, 1]
[1, 1]               [1, 0]
```

---

## 3. CƠ CHẾ KÉO THẢ & BẮT DÍNH (DRAG & DROP & SNAP LOGIC)

### 3.1. Các trạng thái của Vật phẩm (Item States)
1. `InDock`: Nằm yên trong khay chứa dưới đáy màn hình.
2. `Dragging`: Đang được nhấc lên, theo dõi tọa độ ngón tay (Screen Space to World/Local Space), nâng giá trị `Z-index` / `SortingLayer` lên cao nhất, phóng to nhẹ ($\times 1.05$) để người chơi không bị ngón tay che khuất.
3. `Snapping`: Bay mượt mà (Tweening) về ô lưới mục tiêu hoặc bay về khay chứa.
4. `Placed`: Cố định trong ma trận ô của balo.

### 3.2. Chuyển đổi tọa độ (Coordinate Conversion)
Để xác định ô lưới dưới ngón tay khi đang kéo:
1. Lấy vị trí thế giới của điểm neo (Pivot) vật phẩm: $P_{world} = (W_x, W_y)$.
2. Chuyển về tọa độ cục bộ của Lưới: $P_{local} = GridTransform.InverseTransformPoint(P_{world})$.
3. Tính chỉ số hàng và cột tương ứng:
$$col = \text{Round}\left(\frac{P_{local}.x - GridOrigin.x}{CellSize + CellSpacing}\right)$$
$$row = \text{Round}\left(\frac{GridOrigin.y - P_{local}.y}{CellSize + CellSpacing}\right)$$

### 3.3. Kiểm tra tính hợp lệ khi Thả (Bounds & Collision Check)
Hàm `CanPlaceItem(item, targetRow, targetCol)` trả về `true` khi và chỉ khi:
1. **Trong phạm vi:** Với mọi ô $[i, j]$ có giá trị `1` trong ma trận vật phẩm:
   $$0 \le targetRow + i < TotalRows \quad \text{và} \quad 0 \le targetCol + j < TotalCols$$
2. **Không đè ô khác:** Ô tương ứng trên lưới `BackpackGrid[targetRow + i, targetCol + j]` phải đang trống (`null`).

---

## 4. THIẾT KẾ KIẾN TRÚC CLASS (CLASS ARCHITECTURE)

```
        +-------------------+
        |    GameManager    |
        +---------+---------+
                  |
        +---------v---------+
        |  BackpackManager  |<----------+
        +---------+---------+           |
                  |                     |
         +--------v--------+   +--------+--------+
         |   GridSystem    |   | RuleValidator   |
         +--------+--------+   +-----------------+
                  |
       +----------v----------+
       |      ItemView       |
       |  (Drag/Drop Input)  |
       +----------+----------+
                  |
       +----------v----------+
       |      ItemData       |
       | (Shape, Tags, Rules)|
       +---------------------+
```

### 4.1. `ItemData` (ScriptableObject / Data Model)
Chứa dữ liệu tĩnh của vật phẩm:
* `string itemId`: Định danh duy nhất.
* `string itemName`: Tên hiển thị.
* `int[,] baseShape`: Ma trận hình dạng ban đầu.
* `List<ItemTag> tags`: Danh sách nhãn (vd: `ItemTag.Heavy`, `ItemTag.Wet`, `ItemTag.Fragile`).
* `List<RuleDefinition> rules`: Các quy tắc mà vật phẩm này yêu cầu.

### 4.2. `ItemView` (MonoBehaviour / Component điều khiển hiển thị và tương tác)
Quản lý tương tác cảm ứng và trạng thái hiển thị của vật phẩm:
* **Thuộc tính:**
  * `ItemData data`: Tham chiếu dữ liệu gốc.
  * `int[,] currentShape`: Ma trận hình dáng sau khi xoay.
  * `int currentRotationStep`: Số lần xoay $90^\circ$ ($0, 1, 2, 3$).
  * `Vector2Int currentGridCoord`: Tọa độ gốc trên lưới nếu đã được đặt vào.
* **Phương thức:**
  * `OnPointerDown()`: Bắt đầu bấm.
  * `OnDrag()`: Cập nhật vị trí theo ngón tay, gọi `BackpackManager.UpdatePreview(this)`.
  * `OnPointerUp()`: Kết thúc kéo, gọi `BackpackManager.TryPlaceItem(this)`.
  * `Rotate()`: Xoay ma trận hình dạng $90^\circ$ và cập nhật góc xoay sprite (`Transform.localEulerAngles`).

### 4.3. `GridSystem`
Quản lý mảng dữ liệu 2D của chiếc balo:
* **Thuộc tính:**
  * `int rows, cols`: Kích thước lưới.
  * `ItemView[,] cellOccupants`: Mảng 2D lưu trữ vật thể đang chiếm ô.
* **Phương thức:**
  * `bool IsCellEmpty(int r, int c)`: Kiểm tra ô có trống không.
  * `bool CanPlaceAt(int[,] shape, int startR, int startC)`: Kiểm tra không gian trống.
  * `void RegisterItem(ItemView item, int startR, int startC)`: Đánh dấu các ô đã bị chiếm.
  * `void UnregisterItem(ItemView item)`: Xóa vật phẩm khỏi lưới (khi nhấc lên kéo lại).

### 4.4. `BackpackManager`
Trung gian điều phối giữa người chơi, Lưới và Hệ thống Kiểm tra:
* **Thuộc tính:**
  * `GridSystem grid`: Thể hiện lưới hiện tại.
  * `List<ItemView> allItems`: Danh sách toàn bộ vật phẩm trong màn chơi.
* **Phương thức:**
  * `void UpdatePreview(ItemView draggingItem)`: Vẽ màu ô dự kiến (Xanh lá nếu hợp lệ, Đỏ nếu trùng ô hoặc vượt biên).
  * `bool TryPlaceItem(ItemView item)`: Xử lý logic khi nhả tay: Snap vào lưới hoặc hoàn trả về khay.
  * `void CheckWinCondition()`: Kiểm tra nếu tất cả vật phẩm đã vào lưới -> Kích hoạt `RuleValidator`.

---

## 5. BỘ KIỂM TRA QUY TẮC NGẦM (RULE VALIDATOR)

Hệ thống dùng mô hình kiểm tra độc lập (Decoupled Rules). Mỗi quy tắc kế thừa từ một interface cơ sở:

```csharp
public interface IItemRule
{
    // Trả về true nếu hợp lệ, false nếu vi phạm; out string lỗi để hiển thị UI
    bool Validate(ItemView self, GridSystem grid, out string failureReason);
}
```

### 5.1. Triển khai các Quy tắc Cụ thể

#### A. Quy tắc Giữ thẳng đứng (`UprightRule`)
* **Kiểm tra:** `self.currentRotationStep == 0`.
* **Thông báo lỗi:** "Món này sẽ bị đổ/rò rỉ nếu bị nghiêng hoặc lật ngang!"

#### B. Quy tắc Chống Đè Nặng (`NoHeavyAboveRule`)
* **Duyệt:** Với mọi ô $(r, c)$ của `self`, kiểm tra ô ngay phía trên là $(r - 1, c)$.
* **Kiểm tra:** Nếu ô $(r - 1, c)$ tồn tại vật phẩm khác và vật phẩm đó có tag `ItemTag.Heavy` $\to$ Vi phạm.
* **Thông báo lỗi:** "Vật phẩm phía trên quá nặng làm bẹp món này!"

#### C. Quy tắc Tránh Xung Đột Tiếp Xúc (`AvoidAdjacentTagRule`)
* **Tham số:** `ItemTag forbiddenTag` (Ví dụ: `ItemTag.Wet`).
* **Duyệt:** Quét 4 ô lân cận (Up, Down, Left, Right) xung quanh toàn bộ các ô thuộc `self`.
* **Kiểm tra:** Nếu bất kỳ ô lân cận nào chứa vật phẩm mang nhãn `forbiddenTag` $\to$ Vi phạm.
* **Thông báo lỗi:** "Món đồ này đang bị ướt/hư hại bởi vật phẩm tiếp xúc bên cạnh!"

---

## 6. THUẬT TOÁN ĐIỀU KIỆN THẮNG TỔNG THỂ

```csharp
public bool ValidateEntireBackpack()
{
    // 1. Kiểm tra toàn bộ vật phẩm đã được đưa vào balo chưa
    foreach (var item in allItems)
    {
        if (item.CurrentState != ItemState.Placed) 
            return false;
    }

    // 2. Kiểm tra quy tắc của từng vật phẩm
    bool allValid = true;
    foreach (var item in allItems)
    {
        foreach (var rule in item.Data.Rules)
        {
            if (!rule.Validate(item, grid, out string reason))
            {
                allValid = false;
                item.TriggerErrorEffect(reason); // Rung lắc và hiện hiệu ứng vi phạm
            }
        }
    }

    return allValid;
}
```