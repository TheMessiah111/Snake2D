using System;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private int moveSpeed;
    private List<Transform> segments = new List<Transform>();
    public Transform segmentPreFab;

    private int score = 0;

    private Transform trans;
    private Vector2 direction = Vector2.right;
    public static event Action OnSnakeCollision;
    private int difficulty;
    
    // Swipe detection variables
    private Vector2 swipeStartPos;
    private Vector2 swipeEndPos;
    private float minSwipeDistance = 50f; // Minimum distance for a swipe to register
    private bool isSwiping = false;

    private GameManager gameManager;

    private void Start()
    {
        trans = GetComponent<Transform>();
        segments.Add(this.transform);
        difficulty = PlayerPrefs.GetInt("SelectedDifficulty", 0);
        gameManager = FindFirstObjectByType<GameManager>();
        
        switch (difficulty)
        {
            case 0:
                moveSpeed = 5;
                break;
            case 1:
                moveSpeed = 10;
                break;
            case 2:
                moveSpeed = 15;
                break;
            default:
                moveSpeed = 5;
                // Debug.LogWarning("Invalid difficulty, defaulting to Easy");
                break;
        }
    }

    private void Update()
    {
        if(gameManager != null && !gameManager.isGameOver)
        {
            HandleInput();
        }
    }

    private void FixedUpdate()
    {
        // Move tail segments first so they follow the head
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        // Move the head
        if (gameManager != null && !gameManager.isGameOver)
        {
            trans.position = (Vector2)trans.position + direction * moveSpeed * Time.fixedDeltaTime;
        }
    }

    private void HandleInput()
    {
        // Keyboard input
        HandleKeyboardInput();
        
        // Touch/Mouse input for swipes
        HandleSwipeInput();
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2.down)
        {
            ChangeDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2.up)
        {
            ChangeDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2.left)
        {
            ChangeDirection(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2.right)
        {
            ChangeDirection(Vector2.left);
        }
    }

    private void HandleSwipeInput()
    {
        // Touch input (mobile)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                swipeStartPos = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Ended && isSwiping)
            {
                swipeEndPos = touch.position;
                DetectSwipe();
                isSwiping = false;
            }
        }
        // Mouse input (for testing in editor)
        else if (Input.GetMouseButtonDown(0))
        {
            swipeStartPos = Input.mousePosition;
            isSwiping = true;
        }
        else if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            swipeEndPos = Input.mousePosition;
            DetectSwipe();
            isSwiping = false;
        }
    }

    private void DetectSwipe()
    {
        Vector2 swipeDelta = swipeEndPos - swipeStartPos;
        
        // Check if swipe is long enough
        if (swipeDelta.magnitude < minSwipeDistance)
            return;

        // Normalize to get direction
        swipeDelta.Normalize();

        // Determine swipe direction based on the larger component
        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
        {
            // Horizontal swipe
            if (swipeDelta.x > 0 && direction != Vector2.left)
            {
                ChangeDirection(Vector2.right);
            }
            else if (swipeDelta.x < 0 && direction != Vector2.right)
            {
                ChangeDirection(Vector2.left);
            }
        }
        else
        {
            // Vertical swipe
            if (swipeDelta.y > 0 && direction != Vector2.down)
            {
                ChangeDirection(Vector2.up);
            }
            else if (swipeDelta.y < 0 && direction != Vector2.up)
            {
                ChangeDirection(Vector2.down);
            }
        }
    }

    private void ChangeDirection(Vector2 newDirection)
    {
        direction = newDirection;
        // Snap to grid on direction change
        trans.position = new Vector2(Mathf.Round(trans.position.x), Mathf.Round(trans.position.y));
    }

    private void SnakeGrowth()
    {
        Transform segment = Instantiate(this.segmentPreFab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }
    public void ShortenSnake(int amount){
         Debug.Log("Snake hears the call");
    // Remove segments from the end of the snake
    for (int i = 0; i < amount; i++)
    {
        if (this.segments.Count > 1) // Keep at least the head
        {
            Debug.Log("Snake performs the task");
            Destroy(this.segments[this.segments.Count - 1].gameObject);
            this.segments.RemoveAt(this.segments.Count - 1);
        }
    }
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            SnakeGrowth();
            score += 1;
            // Destroy(other.gameObject);
        }
        else if(other.tag == "Boundary" || other.tag == "SnakeSegment")
        {
            // Debug.Log("Game Over - Hit " + other.tag);
            OnSnakeCollision?.Invoke();
        }
    }

    // Public accessor for score
    public int Score => score;
}