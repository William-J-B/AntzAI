using UnityEngine;

public class Anthill : MonoBehaviour
{
    public int gridX { get; private set; }
    public int gridY { get; private set; }
    public int playerId { get; private set; }

    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    }

    public void Initialize(int x, int y, int player, GameManager manager)
    {
        gridX = x;
        gridY = y;
        playerId = player;
        gameManager = manager;
        transform.position = new Vector3(x, y, -1);

        // Set visual appearance
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            // Use player color but darker for anthill
            Color anthillColor = gameManager.GetPlayerColor(playerId);
            anthillColor = Color.Lerp(anthillColor, Color.black, 0.3f);
            spriteRenderer.color = anthillColor;

            // Create a dome-like texture for anthill
            if (spriteRenderer.sprite == null)
            {
                Texture2D texture = new Texture2D(40, 40);
                Color[] pixels = new Color[40 * 40];

                for (int x = 0; x < 40; x++)
                {
                    for (int y = 0; y < 40; y++)
                    {
                        Vector2 center = new Vector2(20, 20);
                        float distance = Vector2.Distance(new Vector2(x, y), center);

                        if (distance <= 18)
                        {
                            // Create dome effect with gradient
                            float intensity = 1f - (distance / 18f);
                            pixels[y * 40 + x] = new Color(1f, 1f, 1f, intensity);
                        }
                        else
                        {
                            pixels[y * 40 + x] = Color.clear;
                        }
                    }
                }

                texture.SetPixels(pixels);
                texture.Apply();

                spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 40, 40), new Vector2(0.5f, 0.5f));
            }
        }

        transform.localScale = Vector3.one * 1.2f;
    }
}