
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Text scoreText;
    private Text healthText;
    private GameObject gameOverPanel;

    private GameObject targetPrefab;
    private float spawnInterval;
    private Rect spawnArea;

    private int score = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void InitializeUI(Text sText, Text hText, GameObject goPanel)
    {
        scoreText = sText;
        healthText = hText;
        gameOverPanel = goPanel;

        gameOverPanel.SetActive(false);
        UpdateScore(0);
        UpdateHealth(10); // 초기 체력은 PendulumController에서 설정되므로 임시값
    }

    public void InitializeGameObjects(PendulumController pendulum, float interval, Rect area)
    {
        targetPrefab = CreateTargetPrefab();
        spawnInterval = interval;
        spawnArea = area;
        StartCoroutine(SpawnTargets());
    }

    public void UpdateScore(int amount)
    {
        score += amount;
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    public void UpdateHealth(int health)
    {
        if (healthText != null) healthText.text = "Health: " + health;
    }

    public void GameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        StopAllCoroutines();
        // Time.timeScale = 0f; // 게임 정지 대신 UI만 표시
    }

    IEnumerator SpawnTargets()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Vector2 spawnPos = new Vector2(
                Random.Range(spawnArea.xMin, spawnArea.xMax),
                Random.Range(spawnArea.yMin, spawnArea.yMax)
            );
            Instantiate(targetPrefab, spawnPos, Quaternion.identity);
        }
    }

    private GameObject CreateTargetPrefab()
    {
        GameObject target = new GameObject("TargetPrefab");
        target.tag = "Target";
        target.transform.localScale = Vector3.one * 0.8f;
        SpriteRenderer sr = target.AddComponent<SpriteRenderer>();
        sr.sprite = CreateCircleSprite(Color.green);
        target.AddComponent<CircleCollider2D>().isTrigger = true;
        Rigidbody2D rb = target.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // 수정됨
        return target;
    }

    Sprite CreateCircleSprite(Color color)
    {
        Texture2D tex = new Texture2D(64, 64);
        tex.wrapMode = TextureWrapMode.Clamp;
        Vector2 center = new Vector2(31.5f, 31.5f);
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                if (dist > 32) tex.SetPixel(x, y, Color.clear);
                else if (dist > 30) tex.SetPixel(x, y, new Color(color.r, color.g, color.b, (32-dist)/2f));
                else tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 100.0f);
    }
}
