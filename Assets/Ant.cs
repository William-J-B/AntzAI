// ==================== Ant.cs ====================
using UnityEngine;

public class Ant : MonoBehaviour
{
    // Remove health and attackDamage
    // public int maxHealth = 3;
    // public int attackDamage = 1;

    // Public properties
    public int gridX { get; private set; }
    public int gridY { get; private set; }
    public int playerId { get; private set; }
    // Remove health
    // public int health { get; private set; }
    public bool hasActed { get; set; }
    public bool isCarryingFood { get; private set; }
    public Sprite blackSprite;
    public Sprite redSprite;

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
        // Remove health
        // health = maxHealth;
        hasActed = false;
        isCarryingFood = false;
        gameManager = manager;

        // Set visual appearance
        UpdateVisual();
    }

    void UpdateVisual()
    {
        // Create a simple colored square sprite
        if (spriteRenderer != null)
        {
            if (playerId == 1)
            {
                spriteRenderer.sprite = blackSprite;
            }
            else
            {
                spriteRenderer.sprite = redSprite;
            }

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

        // Remove health-based scaling
        float baseScale = 0.3f;

        // Make carrying ants slightly larger to show they have food
        if (isCarryingFood)
            baseScale *= 1.2f;

        transform.localScale = Vector3.one * baseScale;
    }

    public void MoveTo(int newX, int newY)
    {
        gridX = newX;
        gridY = newY;
        transform.position = new Vector3(newX, newY, -1);
    }

    // Remove TakeDamage
    // public void TakeDamage(int damage) { ... }

    public void PickupFood()
    {
        isCarryingFood = true;
        UpdateVisual();
    }

    public void DropFood()
    {
        isCarryingFood = false;
        UpdateVisual();
    }
}