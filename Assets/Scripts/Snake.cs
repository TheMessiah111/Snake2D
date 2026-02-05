using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class Snake : MonoBehaviour
{
    private int moveSpeed = 5;
    private List<Transform> segments = new List<Transform>();
    public Transform segmentPreFab;

    private int score = 0;

    private Transform trans;
    private Vector2 direction = Vector2.right;
    public static event Action OnSnakeCollision;

    public Transform LeftBoundary, RightBoundary, TopBoundary, BottomBoundary;
    // private float offset = 0.1f;

    private void Start()
    {
        trans = GetComponent<Transform>();
        segments.Add(this.transform);
        // This sets up initial movement, though it's often better to let it start after the first input.
        // Or you can use this to apply constant velocity.
        // For a grid-based snake, movement is often handled by updating position, not applying velocity.
    }

    private void Update()
    {
        // Handle input to change direction.
        // We use GetKey, not GetKeyDown, so you can hold the button for a single move on a grid.
        {
            if(FindFirstObjectByType<GameManager>().isGameOver == false)
            {
                SnakeMovement();
            }
        }
        
    }

    private void FixedUpdate()
    {
        // Move tail segments first so they follow the head.
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }

        // Move the head.
        // We add the direction vector multiplied by a move speed.
        // For a grid-based game, a better way is to move one unit per tick.
        // For example: transform.position = (Vector2)transform.position + direction;
        if (FindFirstObjectByType<GameManager>().isGameOver == false)
        {
            trans.position = (Vector2)trans.position + direction * moveSpeed * Time.fixedDeltaTime;
        }
    }

private void SnakeMovement()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2.down)
        {
            direction = Vector2.up;
            trans.position = new Vector2(Mathf.Round(trans.position.x), Mathf.Round(trans.position.y));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2.up)
        {
            direction = Vector2.down;
            trans.position = new Vector2(Mathf.Round(trans.position.x), Mathf.Round(trans.position.y));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2.left)
        {
            direction = Vector2.right;
            trans.position = new Vector2(Mathf.Round(trans.position.x), Mathf.Round(trans.position.y));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2.right)
        {
            direction = Vector2.left;
            trans.position = new Vector2(Mathf.Round(trans.position.x), Mathf.Round(trans.position.y));
        }
    }
    private void SnakeGrowth()
    {
        Transform segment = Instantiate(this.segmentPreFab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }
    private void OnCollisionEnter2D(Collision2D other) {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            SnakeGrowth();
            score += 1;
            // FindFirstObjectByType<ScoreUpdate>().UpdateScore(score);
        }
        else if(other.tag == "Boundary")
        {
            Debug.Log("Hit Boundary");

           OnSnakeCollision?.Invoke();
        
        }
       
    }
}