
using UnityEngine;

public class PendulumController : MonoBehaviour
{
    public int maxHealth = 10; // GameInitializer에서 설정
    private int currentHealth;

    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector2 mouseStartPos;
    private Vector2 pendulumStartPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        GameManager.Instance.UpdateHealth(currentHealth);
    }

    void OnMouseDown()
    {
        isDragging = true;
        rb.bodyType = RigidbodyType2D.Kinematic; // 수정됨
        mouseStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pendulumStartPos = transform.position;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newPos = pendulumStartPos + (currentMousePos - mouseStartPos);
            rb.MovePosition(newPos);
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        rb.bodyType = RigidbodyType2D.Dynamic; // 수정됨
        // 마우스 드래그 방향으로 힘을 가하여 진자를 스윙시킵니다.
        Vector2 releaseVector = (Vector2)transform.position - mouseStartPos;
        rb.AddForce(releaseVector * 100); // 힘의 크기 조절
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            GameManager.Instance.UpdateScore(100); // 타겟 맞추면 100점
            Destroy(collision.gameObject); // 타겟 파괴
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            currentHealth--;
            GameManager.Instance.UpdateHealth(currentHealth);
            if (currentHealth <= 0)
            {
                // 진자 파괴 (Bob의 부모인 Boom, Pendulum_Pivot까지 파괴)
                Destroy(transform.parent.parent.gameObject);
                GameManager.Instance.GameOver();
            }
        }
    }
}
