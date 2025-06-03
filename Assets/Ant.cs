// ==================== Ant.cs ====================
using UnityEngine;

public class Ant : MonoBehaviour
{
    [Header("Ant Stats")]
    public int maxHealth = 3;
    public int attackDamage = 1;

    // Public properties
    public int gridX { get; private set; }
    public int gridY { get; private set; }
    public int playerId { get; private set; }
    public int health { get; private set; }
    public bool hasActed { get; set; }

    // Components
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
        health = maxHealth;
        hasActed = false;
        gameManager = manager;

        // Set visual appearance
        UpdateVisual();
    }

    void UpdateVisual()
    {
        // Create a simple colored square sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.color = gameManager.GetPlayerColor(playerId);

            // Create a simple square texture if none exists
            if (spriteRenderer.sprite == null)
            {
                Texture2D texture = new Texture2D(32, 32);
                Color[] pixels = new Color[32 * 32];
                for (int i = 0; i < pixels.Length; i++)
                    pixels[i] = Color.white;
                texture.SetPixels(pixels);
                texture.Apply();

                spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            }
        }

        // Add a simple health indicator (scale based on health)
        float healthRatio = (float)health / maxHealth;
        transform.localScale = Vector3.one * (0.8f + 0.4f * healthRatio);
    }

    public void MoveTo(int newX, int newY)
    {
        gridX = newX;
        gridY = newY;
        transform.position = new Vector3(newX, newY, -1);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(0, health);
        UpdateVisual();
    }
}