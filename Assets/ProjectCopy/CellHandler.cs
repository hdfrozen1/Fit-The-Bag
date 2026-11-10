using UnityEngine;
using UnityEngine.EventSystems;

public class CellHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private ItemDragHandler parentManager;

    void Start()
    {
        // Tự động tìm script quản lý ở Object cha lớn nhất
        parentManager = GetComponentInParent<ItemDragHandler>();
    }

    // --- 1. SỰ KIỆN KÉO THẢ (Truyền lên cha) ---
    public void OnPointerDown(PointerEventData eventData)
    {
        if (parentManager != null) parentManager.HandlePointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentManager != null) parentManager.HandleDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (parentManager != null) parentManager.HandlePointerUp(eventData);
    }

    // --- 2. SỰ KIỆN VA CHẠM TRIGGER (Truyền lên cha) ---
    void OnTriggerEnter2D(Collider2D other)
    {
        if (parentManager != null) 
        {
            // Kiểm tra xem ô con này có đang chạm vào ô con của vật thể KHÁC không
            // (Dùng transform.root để đảm bảo không tự va chạm với các ô con trong chính mình)
            if (other.transform.root != transform.root)
            {
                parentManager.RegisterCollision(other.transform.root.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (parentManager != null)
        {
            if (other.transform.root != transform.root)
            {
                parentManager.UnregisterCollision(other.transform.root.gameObject);
            }
        }
    }
}