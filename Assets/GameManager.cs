// ==================== GameManager.cs ====================
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int gridWidth = 20;
    public int gridHeight = 15;
    public int initialAntsPerPlayer = 10;
    public int initialFoodCount = 6;

    [Header("Prefabs")]
    public GameObject antPrefab;
    public GameObject foodPrefab;
    public GameObject tilePrefab;
    public GameObject anthillPrefab;

    [Header("UI")]
    public Text player1ScoreText;
    public Text player2ScoreText;
    public Text turnIndicatorText;
    public Text gameStatusText;
    public Button restartButton;

    [Header("Colors")]
    public Color player1Color = Color.red;
    public Color player2Color = Color.blue;
    public Color tileColor = Color.white;
    public Color selectedTileColor = Color.yellow;

    // Game State
    private GameState currentState = GameState.Player1Turn;
    private int currentTurn = 1;
    private int player1Score = 0;
    private int player2Score = 0;

    // Grid and Objects
    private GridTile[,] grid;
    private List<Ant> player1Ants = new List<Ant>();
    private List<Ant> player2Ants = new List<Ant>();
    private List<Food> foodItems = new List<Food>();
    private Anthill player1Anthill;
    private Anthill player2Anthill;

    // Selection
    private Ant selectedAnt;
    private Vector2Int selectedPosition;

    // Camera
    private Camera mainCamera;

    public enum GameState
    {
        Player1Turn,
        Player2Turn,
        GameOver
    }

    void Start()
    {
        mainCamera = Camera.main;
        SetupCamera();
        InitializeGame();
    }

    void SetupCamera()
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(gridWidth / 2f, gridHeight / 2f, -10f);
            mainCamera.orthographicSize = Mathf.Max(gridWidth, gridHeight) / 2f + 1f;
        }
    }

    void InitializeGame()
    {
        CreateGrid();
        SpawnAnthills();
        SpawnAnts();
        SpawnFood();
        UpdateUI();

        // Setup UI buttons
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    void CreateGrid()
    {
        grid = new GridTile[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                GridTile tile = tileObj.GetComponent<GridTile>();
                if (tile == null)
                    tile = tileObj.AddComponent<GridTile>();

                tile.Initialize(x, y, this);
                grid[x, y] = tile;
            }
        }
    }

    void SpawnAnthills()
    {
        // Player 1 anthill (left)
        int p1X = 0;
        int p1Y = 7;
        GameObject anthill1 = Instantiate(anthillPrefab, new Vector3(p1X, p1Y, -1), Quaternion.identity);
        player1Anthill = anthill1.GetComponent<Anthill>();
        if (player1Anthill == null)
            player1Anthill = anthill1.AddComponent<Anthill>();
        player1Anthill.Initialize(p1X, p1Y, 1, this);

        // Player 2 anthill (right)
        int p2X = 19;
        int p2Y = 7;
        GameObject anthill2 = Instantiate(anthillPrefab, new Vector3(p2X, p2Y, -1), Quaternion.identity);
        player2Anthill = anthill2.GetComponent<Anthill>();
        if (player2Anthill == null)
            player2Anthill = anthill2.AddComponent<Anthill>();
        player2Anthill.Initialize(p2X, p2Y, 2, this);
    }

    void SpawnAnts()
    {
        // Spawn Player 1 ants (left side)
        if (initialAntsPerPlayer == 10)
        {
            SpawnAnt(1, 9, 1);
            SpawnAnt(1, 5, 1);
            SpawnAnt(2, 11, 1);
            SpawnAnt(2, 7, 1);
            SpawnAnt(2, 3, 1);
            SpawnAnt(3, 10, 1);
            SpawnAnt(3, 4, 1);
            SpawnAnt(4, 6, 1);
            SpawnAnt(4, 8, 1);
            SpawnAnt(5, 7, 1);
        }


        // Spawn Player 2 ants (right side)
        if (initialAntsPerPlayer == 10)
        {
            SpawnAnt(18, 9, 2);
            SpawnAnt(18, 5, 2);
            SpawnAnt(17, 11, 2);
            SpawnAnt(17, 7, 2);
            SpawnAnt(17, 3, 2);
            SpawnAnt(16, 10, 2);
            SpawnAnt(16, 4, 2);
            SpawnAnt(15, 6, 2);
            SpawnAnt(15, 8, 2);
            SpawnAnt(14, 7, 2);
        }
    }

    void SpawnAnt(int x, int y, int playerId)
    {
        GameObject antObj = Instantiate(antPrefab, new Vector3(x, y, -1), Quaternion.identity);
        Ant ant = antObj.GetComponent<Ant>();
        if (ant == null)
            ant = antObj.AddComponent<Ant>();

        ant.Initialize(x, y, playerId, this);

        if (playerId == 1)
            player1Ants.Add(ant);
        else
            player2Ants.Add(ant);
    }

    void SpawnFood()
    {
        if (initialFoodCount == 6)
        {
            CreateFood(8, 5);
            CreateFood(8, 9);
            CreateFood(9, 7);
            CreateFood(10, 7);
            CreateFood(11, 5);
            CreateFood(11, 9);
        }
    }

    void CreateFood(int x, int y)
    {
        GameObject foodObj = Instantiate(foodPrefab, new Vector3(x, y, -1), Quaternion.identity);
        Food food = foodObj.GetComponent<Food>();
        if (food == null)
            food = foodObj.AddComponent<Food>();

        food.Initialize(x, y);
        foodItems.Add(food);
    }

    void Update()
    {
        if (currentState == GameState.GameOver) return;

        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(worldPos.x);
            int y = Mathf.RoundToInt(worldPos.y);

            if (IsValidPosition(x, y))
            {
                HandleTileClick(x, y);
            }
        }
    }

    void HandleTileClick(int x, int y)
    {
        Ant clickedAnt = GetAntAt(x, y);

        if (clickedAnt != null && clickedAnt.playerId == GetCurrentPlayerId() && !clickedAnt.hasActed)
        {
            // Select ant
            SelectAnt(clickedAnt);
        }
        else if (selectedAnt != null)
        {
            // Try to move or attack
            if (CanMoveAnt(selectedAnt, x, y))
            {
                MoveAnt(selectedAnt, x, y);
            }
            else if (CanAttack(selectedAnt, x, y))
            {
                AttackAnt(selectedAnt, x, y);
            }
        }
    }

    void SelectAnt(Ant ant)
    {
        // Deselect previous
        if (selectedAnt != null)
        {
            HighlightTile(selectedAnt.gridX, selectedAnt.gridY, false);
        }

        selectedAnt = ant;
        selectedPosition = new Vector2Int(ant.gridX, ant.gridY);

        // Highlight selected ant
        HighlightTile(ant.gridX, ant.gridY, true);
    }

    bool CanMoveAnt(Ant ant, int targetX, int targetY)
    {
        if (!IsValidPosition(targetX, targetY)) return false;
        if (ant.hasActed) return false;

        // Check if adjacent
        int dx = Mathf.Abs(targetX - ant.gridX);
        int dy = Mathf.Abs(targetY - ant.gridY);
        if (dx + dy != 1) return false;

        // Check if occupied
        if (GetAntAt(targetX, targetY) != null) return false;

        return true;
    }

    bool CanAttack(Ant attacker, int targetX, int targetY)
    {
        if (!IsValidPosition(targetX, targetY)) return false;
        if (attacker.hasActed) return false;

        // Check if adjacent
        int dx = Mathf.Abs(targetX - attacker.gridX);
        int dy = Mathf.Abs(targetY - attacker.gridY);
        if (dx + dy != 1) return false;

        // Check if enemy ant is there
        Ant target = GetAntAt(targetX, targetY);
        return target != null && target.playerId != attacker.playerId;
    }

    void MoveAnt(Ant ant, int targetX, int targetY)
    {
        ant.MoveTo(targetX, targetY);

        // Check for food collection
        Food food = GetFoodAt(targetX, targetY);
        if (food != null && !ant.isCarryingFood)
        {
            PickupFood(ant, food);
        }

        Anthill anthill = GetAnthillAt(targetX, targetY);
        if (anthill != null && anthill.playerId == ant.playerId && ant.isCarryingFood)
        {
            DeliverFood(ant, anthill);
        }

        ant.hasActed = true;
        EndTurn();
        DeselectAnt();
    }

    void AttackAnt(Ant attacker, int targetX, int targetY)
    {
        Ant target = GetAntAt(targetX, targetY);
        if (target != null)
        {
            target.TakeDamage(attacker.attackDamage);
            attacker.hasActed = true;

            if (target.health <= 0)
            {
                DestroyAnt(target);
            }
        }
        DeselectAnt();
    }

    void PickupFood(Ant ant, Food food)
    {
        ant.PickupFood();
        foodItems.Remove(food);
        Destroy(food.gameObject);

        // Don't score yet - must deliver to anthill first!
        UpdateUI();
    }

    void DeliverFood(Ant ant, Anthill anthill)
    {
        ant.DropFood();

        // Now we score!
        if (ant.playerId == 1)
            player1Score++;
        else
            player2Score++;

        UpdateUI();
        CheckGameEnd();
    }

    void DestroyAnt(Ant ant)
    {
        if (ant.playerId == 1)
            player1Ants.Remove(ant);
        else
            player2Ants.Remove(ant);

        if (selectedAnt == ant)
            DeselectAnt();

        Destroy(ant.gameObject);
        CheckGameEnd();
    }

    void DeselectAnt()
    {
        if (selectedAnt != null)
        {
            HighlightTile(selectedAnt.gridX, selectedAnt.gridY, false);
            selectedAnt = null;
        }
    }

    void HighlightTile(int x, int y, bool highlight)
    {
        if (IsValidPosition(x, y))
        {
            grid[x, y].SetHighlight(highlight);
        }
    }

    public void EndTurn()
    {
        DeselectAnt();

        // Reset all ants' action status
        List<Ant> currentPlayerAnts = GetCurrentPlayerAnts();
        foreach (Ant ant in currentPlayerAnts)
        {
            ant.hasActed = false;
        }

        // Switch turns
        if (currentState == GameState.Player1Turn)
            currentState = GameState.Player2Turn;
        else
        {
            currentState = GameState.Player1Turn;
            currentTurn++;
        }

        UpdateUI();
        CheckGameEnd();
    }

    void CheckGameEnd()
    {
        bool gameEnded = false;
        string winner = "";

        // Check if all food collected
        if (foodItems.Count == 0)
        {
            gameEnded = true;
            winner = player1Score > player2Score ? "Player 1" :
                     player2Score > player1Score ? "Player 2" : "Tie";
        }
        // Check if all ants of one player are dead
        else if (player1Ants.Count == 0)
        {
            gameEnded = true;
            winner = "Player 2";
        }
        else if (player2Ants.Count == 0)
        {
            gameEnded = true;
            winner = "Player 1";
        }

        if (gameEnded)
        {
            currentState = GameState.GameOver;
            if (gameStatusText != null)
                gameStatusText.text = winner == "Tie" ? "Game Over - Tie!" : $"Game Over - {winner} Wins!";
        }
    }

    void UpdateUI()
    {
        if (player1ScoreText != null)
            player1ScoreText.text = $"Player 1: {player1Score}";
        if (player2ScoreText != null)
            player2ScoreText.text = $"Player 2: {player2Score}";
        if (turnIndicatorText != null)
            turnIndicatorText.text = $"Turn {currentTurn} - {(currentState == GameState.Player1Turn ? "Player 1" : "Player 2")}";
        if (gameStatusText != null && currentState != GameState.GameOver)
            gameStatusText.text = "";
    }

    public void RestartGame()
    {
        // Clear existing objects
        foreach (Ant ant in player1Ants.ToList())
            if (ant != null) Destroy(ant.gameObject);
        foreach (Ant ant in player2Ants.ToList())
            if (ant != null) Destroy(ant.gameObject);
        foreach (Food food in foodItems.ToList())
            if (food != null) Destroy(food.gameObject);
        if (player1Anthill != null) Destroy(player1Anthill.gameObject);
        if (player2Anthill != null) Destroy(player2Anthill.gameObject);

        player1Ants.Clear();
        player2Ants.Clear();
        foodItems.Clear();

        // Reset game state
        currentState = GameState.Player1Turn;
        currentTurn = 1;
        player1Score = 0;
        player2Score = 0;
        selectedAnt = null;

        // Reinitialize
        SpawnAnthills();
        SpawnAnts();
        SpawnFood();
        UpdateUI();
    }

    // Utility methods
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
    }

    public Ant GetAntAt(int x, int y)
    {
        return player1Ants.FirstOrDefault(a => a.gridX == x && a.gridY == y) ??
               player2Ants.FirstOrDefault(a => a.gridX == x && a.gridY == y);
    }

    public Food GetFoodAt(int x, int y)
    {
        return foodItems.FirstOrDefault(f => f.gridX == x && f.gridY == y);
    }

    public Anthill GetAnthillAt(int x, int y)
    {
        if (player1Anthill != null && player1Anthill.gridX == x && player1Anthill.gridY == y)
            return player1Anthill;
        if (player2Anthill != null && player2Anthill.gridX == x && player2Anthill.gridY == y)
            return player2Anthill;
        return null;
    }

    public int GetCurrentPlayerId()
    {
        return currentState == GameState.Player1Turn ? 1 : 2;
    }

    public List<Ant> GetCurrentPlayerAnts()
    {
        return currentState == GameState.Player1Turn ? player1Ants : player2Ants;
    }

    public Color GetPlayerColor(int playerId)
    {
        return playerId == 1 ? player1Color : player2Color;
    }
}