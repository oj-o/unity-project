
using UnityEngine;

public class LoseZone : MonoBehaviour
{
    [HideInInspector] public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            gameManager.LoseLife();
        }
    }
}
