using System;
using UnityEngine;
public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;
    private GameManager gameManager;
    private int difficulty;
    

    private void Start()
    {
        if (gameManager == null)
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }
     RandomizeFoodPosition();
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
            RandomizeFoodPosition();
            gameManager.UpdateScore();
        }
    }

}
