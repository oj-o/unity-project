using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    // --- 게임 시작 시 자동으로 이 코드를 실행합니다 ---
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeGame()
    {
        // PendulumGame 씬에서만 초기화되도록 조건 추가
        if (SceneManager.GetActiveScene().name == "PendulumGame")
        {
            if (FindAnyObjectByType<GameInitializer>() == null)
            {
                new GameObject("GameInitializer").AddComponent<GameInitializer>();
            }
        }
    }
    // ------------------------------------------------

    [Header("Game Settings")]
    public int pendulumInitialHealth = 10;
    public float targetSpawnInterval = 2f;
    public Rect targetSpawnArea = new Rect(-7, 0, 14, 4);

    void Awake()
    {
        SetupCamera();

        GameObject gmObj = new GameObject("GameManager");
        GameManager gameManager = gmObj.AddComponent<GameManager>();

        Text scoreText = CreateText("ScoreText", "Score: 0", new Vector2(-100, -30), null);
        Text healthText = CreateText("HealthText", "Health: 10", new Vector2(100, -30), null);
        GameObject gameOverPanel = CreatePanel("GameOverPanel", "GAME OVER", null);
        gameManager.InitializeUI(scoreText, healthText, gameOverPanel);

        PendulumController pendulum = CreatePendulum();

        CreateWalls();

        gameManager.InitializeGameObjects(pendulum, targetSpawnInterval, targetSpawnArea);
    }

    void SetupCamera()
    {
        // 기존 카메라 모두 제거
        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera existingCam in allCameras)
        {
            Destroy(existingCam.gameObject);
        }

        // 새로운 메인 카메라 생성
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.tag = "MainCamera"; // 메인 카메라 태그 지정

        cam.transform.position = new Vector3(0, 0, -10);
        cam.orthographic = true;
        cam.orthographicSize = 5.5f;
        cam.backgroundColor = Color.black;
        cam.cullingMask = ~0; // 모든 레이어 렌더링
    }

    PendulumController CreatePendulum()
    {
        GameObject pivot = new GameObject("Pendulum_Pivot");
        pivot.transform.position = new Vector2(0, 4f);
        Rigidbody2D pivotRb = pivot.AddComponent<Rigidbody2D>();
        pivotRb.bodyType = RigidbodyType2D.Static;

        GameObject boom = new GameObject("Boom");
        boom.transform.SetParent(pivot.transform);
        boom.transform.localPosition = Vector2.zero;
        boom.transform.localScale = new Vector2(0.3f, 3f);
        SpriteRenderer boomSr = boom.AddComponent<SpriteRenderer>();
        boomSr.sprite = CreateSquareSprite(Color.gray);
        boomSr.sortingOrder = -1;
        Rigidbody2D boomRb = boom.AddComponent<Rigidbody2D>();

        HingeJoint2D hinge = boom.AddComponent<HingeJoint2D>();
        hinge.connectedBody = pivotRb;
        hinge.anchor = Vector2.zero;
        hinge.connectedAnchor = Vector2.zero;

        GameObject bob = new GameObject("Bob");
        bob.transform.SetParent(boom.transform);
        bob.transform.localPosition = new Vector2(0, -0.5f);
        bob.transform.localScale = new Vector2(1.5f, 1.5f);
        SpriteRenderer bobSr = bob.AddComponent<SpriteRenderer>();
        bobSr.sprite = CreateCircleSprite(Color.red);
        Rigidbody2D bobRb = bob.AddComponent<Rigidbody2D>();
        bobRb.mass = 5f;
        CircleCollider2D bobCollider = bob.AddComponent<CircleCollider2D>();
        bobCollider.radius = 0.5f;

        FixedJoint2D fixedJoint = bob.AddComponent<FixedJoint2D>();
        fixedJoint.connectedBody = boomRb;

        PendulumController controller = bob.AddComponent<PendulumController>();
        controller.maxHealth = pendulumInitialHealth;
        return controller;
    }

    void CreateWalls()
    {
        GameObject topWall = CreateSquareSpriteObject("TopWall", new Vector2(0, 5.5f), new Vector2(18, 1), Color.blue);
        topWall.AddComponent<BoxCollider2D>();
        topWall.AddComponent<Wall>();
        topWall.tag = "Wall";

        GameObject bottomWall = CreateSquareSpriteObject("BottomWall", new Vector2(0, -5.5f), new Vector2(18, 1), Color.blue);
        bottomWall.AddComponent<BoxCollider2D>();
        bottomWall.AddComponent<Wall>();
        bottomWall.tag = "Wall";

        GameObject leftWall = CreateSquareSpriteObject("LeftWall", new Vector2(-9.5f, 0), new Vector2(1, 12), Color.blue);
        leftWall.AddComponent<BoxCollider2D>();
        leftWall.AddComponent<Wall>();
        leftWall.tag = "Wall";

        GameObject rightWall = CreateSquareSpriteObject("RightWall", new Vector2(9.5f, 0), new Vector2(1, 12), Color.blue);
        rightWall.AddComponent<BoxCollider2D>();
        rightWall.AddComponent<Wall>();
        rightWall.tag = "Wall";
    }

    Text CreateText(string name, string content, Vector2 anchoredPos, Transform parent)
    {
        GameObject textObj = new GameObject(name);
        Canvas canvas = FindAnyObjectByType<Canvas>(); 
        if (canvas == null) 
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        textObj.transform.SetParent(canvas.transform);

        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = content;
        text.fontSize = 24;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = anchoredPos;
        return text;
    }

    GameObject CreatePanel(string name, string message, Transform parent)
    {
        GameObject panelObj = new GameObject(name);
        Canvas canvas = FindAnyObjectByType<Canvas>(); 
        if (canvas == null) 
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        panelObj.transform.SetParent(canvas.transform);

        panelObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        RectTransform rect = panelObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Text msgText = CreateText("Message", message, Vector2.zero, panelObj.transform);
        msgText.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        msgText.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        msgText.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

        GameObject buttonObj = new GameObject("RestartButton");
        buttonObj.transform.SetParent(panelObj.transform);
        buttonObj.AddComponent<Image>().color = Color.blue; // 버튼 색상 변경
        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        RectTransform btnRect = buttonObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(160, 40);
        btnRect.anchoredPosition = new Vector2(0, -60);
        CreateText("ButtonText", "Restart", Vector2.zero, buttonObj.transform).color = Color.white; // 텍스트 색상 변경

        panelObj.SetActive(false);
        return panelObj;
    }

    Sprite CreateSquareSprite(Color color)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
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

    GameObject CreateSquareSpriteObject(string name, Vector2 position, Vector2 scale, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateSquareSprite(color);
        return obj;
    }
}