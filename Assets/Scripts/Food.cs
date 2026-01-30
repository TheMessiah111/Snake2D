using System;
using UnityEngine;
public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;

    private void Start()
    {
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
        if (other.tag == "Player")
        {
            RandomizeFoodPosition();
        }
    }

}
