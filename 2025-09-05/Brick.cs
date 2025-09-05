using UnityEngine;

public class Brick : MonoBehaviour
{
    [HideInInspector] public GameManager gameManager;
    public int scoreValue = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            gameManager.AddScore(scoreValue);
            gameManager.DecrementBrickCount();
            Destroy(gameObject);
        }
    }
}