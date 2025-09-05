using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject brickPrefab;
    public int rows = 5;
    public int cols = 10;
    public float spacing = 0.1f;

    void Start()
    {
        GenerateBricks();
    }

    void GenerateBricks()
    {
        if (brickPrefab == null) 
        {
            Debug.LogError("Brick Prefab is not assigned in LevelGenerator!");
            return;
        }

        Vector2 brickSize = brickPrefab.GetComponent<SpriteRenderer>().bounds.size;
        Vector2 startPos = new Vector2(-((cols * brickSize.x) + ((cols - 1) * spacing)) / 2f + brickSize.x/2f, 4f);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector2 pos = new Vector2(
                    startPos.x + col * (brickSize.x + spacing),
                    startPos.y - row * (brickSize.y + spacing)
                );
                Instantiate(brickPrefab, pos, Quaternion.identity, transform);
            }
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetBrickCount(rows * cols);
        }
    }
}