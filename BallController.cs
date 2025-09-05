using UnityEngine;

public class BallController : MonoBehaviour
{
    [HideInInspector] public GameManager gameManager;
    [HideInInspector] public Transform paddleTransform;

    private Rigidbody2D rb;
    private bool isStarted = false;
    private float speed = 8f;
    private Vector3 paddleOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        paddleOffset = transform.position - paddleTransform.position;
        ResetBall();
    }

    void Update()
    {
        if (!isStarted)
        {
            transform.position = paddleTransform.position + paddleOffset;
            if (Input.GetMouseButtonDown(0))
            {
                isStarted = true;
                rb.bodyType = RigidbodyType2D.Dynamic; // 수정된 부분
                rb.velocity = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized * speed;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D col) 
    {
        if(rb.velocity.magnitude < speed * 0.8f)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    public void ResetBall()
    {
        isStarted = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // 수정된 부분
    }
}