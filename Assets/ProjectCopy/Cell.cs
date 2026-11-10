using UnityEngine;

public class Cell : MonoBehaviour
{
    [Tooltip("1 = full , 0 = empty")]
    public int Value = 0;
    [Header("Color Setting")]
    [SerializeField]private Color highLightColor;
    [SerializeField]private Color normalColor;
    private SpriteRenderer spriteRenderer;
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalColor = spriteRenderer.color;
    }
    public void ChangeColorToAvaiable()
    {
        spriteRenderer.color = highLightColor;
    }
    public void ChangeColorToNormal()
    {
        spriteRenderer.color = normalColor;
    }
}
