using UnityEngine;

public class PaddleController : MonoBehaviour
{
    private float speed = 15f;
    private float minX = -8f;
    private float maxX = 8f;
    private Vector3 initialPosition;
    private Camera mainCamera;

    void Start()
    {
        initialPosition = transform.position;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Time.timeScale > 0f)
        {
            float mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition).x;
            float newX = Mathf.Clamp(mousePos, minX, maxX);
            transform.position = new Vector2(newX, transform.position.y);
        }
    }

    public void ResetPaddle()
    {
        transform.position = initialPosition;
    }
}