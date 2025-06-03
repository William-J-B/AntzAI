// ==================== GridTile.cs ====================
using UnityEngine;

public class GridTile : MonoBehaviour
{
    public int gridX { get; private set; }
    public int gridY { get; private set; }

    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;
    private bool isHighlighted = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    }

    public void Initialize(int x, int y, GameManager manager)
    {
        gridX = x;
        gridY = y;
        gameManager = manager;

        // Set visual appearance
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isHighlighted ? gameManager.selectedTileColor : gameManager.tileColor;

            // Create a simple square outline texture
            if (spriteRenderer.sprite == null)
            {
                Texture2D texture = new Texture2D(32, 32);
                Color[] pixels = new Color[32 * 32];

                for (int x = 0; x < 32; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        // Create border
                        if (x == 0 || x == 31 || y == 0 || y == 31)
                            pixels[y * 32 + x] = Color.gray;
                        else
                            pixels[y * 32 + x] = new Color(1, 1, 1, 0.1f); // Semi-transparent white
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            }
        }
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        UpdateVisual();
    }
}