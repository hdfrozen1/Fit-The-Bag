### NGUYÊN TẮC CỐT LÕI (CORE PRINCIPLES)

## 1. TUÂN THỦ S.O.L.I.D & DESIGN PATTERNS:

- Single Responsibility: Một Class chỉ làm đúng một nhiệm vụ. Tách logic UI ra khỏi logic Gameplay.

- Dependency Inversion: Giảm sự phụ thuộc chặt chẽ (Tight Coupling). Sử dụng Interface hoặc Abstract Class để giao tiếp giữa các module.

- Design Patterns: Tự động nhận diện và áp dụng các pattern phù hợp từ sách "Game Programming Patterns" (Nystrom) như:

    + Observer Pattern (C# Events/Actions) để xử lý sự kiện.

    + State Pattern cho logic nhân vật/AI phức tạp.

    + Command Pattern cho hệ thống Input/Replay.

    + Singleton (Dùng hạn chế, ưu tiên ScriptableObject Architecture hoặc Service Locator).



## 2. TỐI ƯU HÓA MOBILE (MOBILE OPTIMIZATION FIRST):

- Zero Garbage Collection (GC) in Update: Tuyệt đối không tạo rác bộ nhớ trong vòng lặp Update().

    + Không dùng LINQ, foreach (trên List cũ), hoặc string concatenation trong Update.

    + Sử dụng Object Pooling cho mọi đối tượng sinh ra/hủy đi liên tục (đạn, effect, enemy).

- Caching: Luôn cache các tham chiếu (GetComponent, Camera.main, Transform) tại Awake/Start.

- Data Structures: Chọn cấu trúc dữ liệu phù hợp (Array/List vs Dictionary) để tối ưu CPU. Sử dụng Struct thay vì Class cho các dữ liệu nhỏ, tồn tại ngắn hạn.

- Physics: Sử dụng Layers collision matrix, hạn chế MeshCollider, ưu tiên Primitive Colliders.



## 3. CODE STYLE & CONVENTIONS (UNITY STANDARD):

- Biến private dùng _camelCase với [SerializeField] thay vì public.

- Property và Method dùng PascalCase.

- Luôn sử dụng RequireComponent nếu class phụ thuộc vào component khác.

- Sử dụng [Tooltip] và [Header] để Inspector dễ nhìn.