using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D clickCursor;

    [Header("HotSpot")]
    [SerializeField] private bool useCenterHotSpot = true;
    [SerializeField] private Vector2 customHotSpot = Vector2.zero;

    private Vector2 hotSpot;
    private bool isClicking;

    private void Start()
    {
        CalcHotSpot();
        SetCursor(normalCursor);
    }

    private void Update()
    {
        bool down = Input.GetMouseButton(0);

        if (down == isClicking) return;
        isClicking = down;

        SetCursor(isClicking ? clickCursor : normalCursor);
    }

    private void CalcHotSpot()
    {
        if (!useCenterHotSpot)
        {
            hotSpot = customHotSpot;
            return;
        }

        // 커서마다 크기가 다를 수 있으니 normal 기준으로 잡되,
        // 가능하면 두 텍스처 크기를 동일하게 맞추는 걸 추천
        if (normalCursor != null)
            hotSpot = new Vector2(normalCursor.width * 0.5f, normalCursor.height * 0.5f);
        else
            hotSpot = Vector2.zero;
    }

    private void SetCursor(Texture2D tex)
    {
        if (tex == null) return;
        Cursor.SetCursor(tex, hotSpot, CursorMode.Auto);
    }
}
