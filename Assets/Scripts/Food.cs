using System;
using UnityEngine;

public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;
    private GameManager gameManager;
    private int difficulty;
    
    [Header("Food Type")]
    public bool isGoldApple = false;
    
    [Header("Gold Apple Settings")]
    [Tooltip("Chance (0-100%) for gold apple to spawn when regular food is eaten")]
    [Range(0, 100)]
    public float goldAppleSpawnChance = 20f;
    
    [Tooltip("Prefab of the gold apple (with isGoldApple = true)")]
    public GameObject goldApplePrefab;
    
    [Tooltip("Points for gold apple")]
    public int goldApplePoints = 7;
    
    [Tooltip("How many segments to remove when eating gold apple")]
    public int segmentsToRemove = 1;
    
    [Tooltip("How long the gold apple stays on screen before disappearing (in seconds)")]
    public float goldAppleLifetime = 5f;
    
    private GameObject currentGoldApple;
    private Snake snake;
    private float lifetimeTimer;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
        
        if (snake == null)
        {
            snake = FindFirstObjectByType<Snake>();
        }
        
        // Only randomize position if this is the regular food
        if (!isGoldApple)
        {
            RandomizeFoodPosition();
        }
        else
        {
            // Start the lifetime timer for gold apples
            lifetimeTimer = goldAppleLifetime;
        }
    }
    
    private void Update()
    {
        // Count down lifetime for gold apples
        if (isGoldApple)
        {
            lifetimeTimer -= Time.deltaTime;
            
            if (lifetimeTimer <= 0)
            {
                // Time's up, destroy the gold apple
                Destroy(gameObject);
            }
        }
    }
    
    private void RandomizeFoodPosition()
    {
        Bounds bounds = this.gridArea.bounds;
        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Snake") && gameManager != null && !gameManager.isGameOver)
        {
            if (isGoldApple)
            {
                // Gold apple behavior
                if (snake != null)
                {
                    snake.ShortenSnake(segmentsToRemove);
                }
                gameManager.UpdateScore(goldApplePoints);
                
                // Destroy the gold apple
                Destroy(gameObject);
            }
            else
            {
                // Regular food behavior
                RandomizeFoodPosition();
                gameManager.UpdateScore();
                
                // Random chance to spawn gold apple
                TrySpawnGoldApple();
            }
        }
    }
    
    private void TrySpawnGoldApple()
    {
        // Only spawn if there isn't already a gold apple and we have the prefab
        if (currentGoldApple == null && goldApplePrefab != null)
        {
            float randomValue = UnityEngine.Random.Range(0f, 100f);
            
            if (randomValue <= goldAppleSpawnChance)
            {
                SpawnGoldApple();
            }
        }
    }
    
    private void SpawnGoldApple()
    {
        Bounds bounds = this.gridArea.bounds;
        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
        
        Vector3 spawnPosition = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
        
        // Make sure it doesn't spawn on the regular food
        int attempts = 0;
        while (Vector3.Distance(spawnPosition, this.transform.position) < 1f && attempts < 100)
        {
            x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);
            spawnPosition = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
            attempts++;
        }
        
        currentGoldApple = Instantiate(goldApplePrefab, spawnPosition, Quaternion.identity);
    }
    
    private void OnDestroy()
    {
        // Clean up reference when destroyed
        if (isGoldApple && currentGoldApple == this.gameObject)
        {
            currentGoldApple = null;
        }
    }
}