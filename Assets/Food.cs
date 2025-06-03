// ==================== Food.cs ====================
using UnityEngine;

public class Food : MonoBehaviour
{
    public int gridX { get; private set; }
    public int gridY { get; private set; }

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    }

    public void Initialize(int x, int y)
    {
        gridX = x;
        gridY = y;
        transform.position = new Vector3(x, y, -1);

        // Set visual appearance
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;

            // Create a simple circle-like texture
            if (spriteRenderer.sprite == null)
            {
                Texture2D texture = new Texture2D(24, 24);
                Color[] pixels = new Color[24 * 24];

                for (int x = 0; x < 24; x++)
                {
                    for (int y = 0; y < 24; y++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(12, 12));
                        pixels[y * 24 + x] = distance <= 10 ? Color.white : Color.clear;
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 24, 24), new Vector2(0.5f, 0.5f));
            }
        }

        transform.localScale = Vector3.one * 0.6f;
    }
}