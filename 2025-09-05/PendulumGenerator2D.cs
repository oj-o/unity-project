using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PendulumGenerator2D : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        if (SceneManager.GetActiveScene().name == "PendulumSim2DMulti")
        {
            if (FindAnyObjectByType<PendulumGenerator2D>() == null)
            {
                new GameObject("PendulumGenerator2D").AddComponent<PendulumGenerator2D>();
            }
        }
    }

    [Header("Pendulum Settings")]
    public int numberOfPendulums = 20;
    public float basePendulumLength = 4f;
    public float lengthRandomness = 1.5f;
    public float initialAngle = 80f;

    [Header("Bob Settings")]
    public float bobMass = 1f;
    public float bobRadius = 0.3f;

    [Header("Visuals")]
    public Color rodColor = Color.white;
    public Color bobColor = new Color(1, 0.5f, 0); // Orange

    void Awake()
    {
        SetupCamera();
        GeneratePendulums();
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
        cam.tag = "MainCamera";

        cam.transform.position = new Vector3(0, 0, -10);
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.backgroundColor = Color.black;
        cam.cullingMask = ~0; // 모든 레이어 렌더링
    }

    void GeneratePendulums()
    {
        Vector2 pivotPos = new Vector2(0, 4f);
        GameObject pivot = new GameObject("SinglePivot");
        pivot.transform.position = pivotPos;
        pivot.transform.SetParent(this.transform);
        Rigidbody2D pivotRb = pivot.AddComponent<Rigidbody2D>();
        pivotRb.bodyType = RigidbodyType2D.Static;

        Material rodMat = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

        for (int i = 0; i < numberOfPendulums; i++)
        {
            GameObject bob = new GameObject($"Bob_{i}");
            
            float currentLength = basePendulumLength + Random.Range(-lengthRandomness, lengthRandomness);
            if (currentLength < 1f) currentLength = 1f;

            float angleRad = initialAngle * Mathf.Deg2Rad;
            Vector2 bobPos = pivotPos + new Vector2(Mathf.Sin(angleRad) * currentLength, -Mathf.Cos(angleRad) * currentLength);
            bob.transform.position = bobPos;
            bob.transform.localScale = Vector3.one * bobRadius * 2;
            
            Color currentBobColor = Color.HSVToRGB((float)i / numberOfPendulums, 0.8f, 1f);
            bob.AddComponent<SpriteRenderer>().sprite = CreateCircleSprite(currentBobColor);
            
            Rigidbody2D bobRb = bob.AddComponent<Rigidbody2D>();
            bobRb.mass = bobMass;

            HingeJoint2D hinge = pivot.AddComponent<HingeJoint2D>();
            hinge.connectedBody = bobRb;
            hinge.enableCollision = false;

            GameObject rod = new GameObject($"Rod_{i}");
            rod.transform.SetParent(this.transform);
            LineRenderer lr = rod.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = 0.03f;
            lr.endWidth = 0.03f;
            lr.material = rodMat;
            lr.startColor = rodColor;
            lr.endColor = rodColor;
            lr.sortingOrder = -1;

            RodUpdater updater = rod.AddComponent<RodUpdater>();
            updater.Initialize(pivot.transform, bob.transform, lr);
        }
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

public class RodUpdater : MonoBehaviour
{
    private Transform pivot;
    private Transform bob;
    private LineRenderer lineRenderer;

    public void Initialize(Transform p, Transform b, LineRenderer lr)
    {
        pivot = p;
        bob = b;
        lineRenderer = lr;
    }

    void Update()
    {
        if (pivot != null && bob != null)
        {
            lineRenderer.SetPosition(0, pivot.position);
            lineRenderer.SetPosition(1, bob.position);
        }
    }
}