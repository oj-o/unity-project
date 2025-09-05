using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PendulumGenerator : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        if (SceneManager.GetActiveScene().name == "PendulumSim3D")
        {
            if (FindAnyObjectByType<PendulumGenerator>() == null)
            {
                new GameObject("PendulumGenerator").AddComponent<PendulumGenerator>();
            }
        }
    }

    [Header("Pendulum Settings")]
    [Range(1, 100)]
    public int numberOfPendulums = 15;
    public float pendulumLength = 5f;
    public float spacing = 1.5f;
    public float initialAngle = 45f;

    [Header("Bob Settings")]
    public float bobMass = 1f;
    public float bobRadius = 0.5f;

    [Header("Visuals")]
    public Color rodColor = Color.white;
    public Color bobColor = Color.cyan;

    private List<GameObject> bobs = new List<GameObject>();
    private List<GameObject> pivots = new List<GameObject>();
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    void Awake()
    {
        SetupCamera();
        SetupLighting();
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

        cam.transform.position = new Vector3(0, 2, -20);
        cam.orthographic = false; // 3D이므로 원근 투영
        cam.fieldOfView = 60; // 시야각
        cam.backgroundColor = Color.black;
        cam.cullingMask = ~0; // 모든 레이어 렌더링
    }

    void SetupLighting()
    {
        // 씬에 조명이 없으면 기본 조명 추가
        if (FindObjectOfType<Light>() == null)
        {
            GameObject lightGameObject = new GameObject("Directional Light");
            Light lightComp = lightGameObject.AddComponent<Light>();
            lightComp.type = LightType.Directional;
            lightGameObject.transform.rotation = Quaternion.Euler(50, -30, 0);
        }
    }

    void GeneratePendulums()
    {
        float totalWidth = (numberOfPendulums - 1) * spacing;
        Vector3 startPosition = new Vector3(-totalWidth / 2f, transform.position.y + 5f, transform.position.z);

        Material rodMat = new Material(Shader.Find("Standard"));
        rodMat.color = rodColor;
        Material bobMat = new Material(Shader.Find("Standard"));
        bobMat.color = bobColor;

        for (int i = 0; i < numberOfPendulums; i++)
        {
            Vector3 pivotPos = startPosition + new Vector3(i * spacing, 0, 0);
            GameObject pivot = new GameObject($"Pivot_{i}");
            pivot.transform.position = pivotPos;
            pivot.transform.SetParent(this.transform);
            Rigidbody pivotRb = pivot.AddComponent<Rigidbody>();
            pivotRb.isKinematic = true;
            pivots.Add(pivot);

            GameObject bob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bob.name = $"Bob_{i}";
            float angleRad = initialAngle * Mathf.Deg2Rad;
            Vector3 bobPos = pivotPos + new Vector3(Mathf.Sin(angleRad) * pendulumLength, -Mathf.Cos(angleRad) * pendulumLength, 0);
            bob.transform.position = bobPos;
            bob.transform.localScale = Vector3.one * bobRadius * 2;
            bob.GetComponent<Renderer>().material = bobMat;
            
            Rigidbody bobRb = bob.AddComponent<Rigidbody>();
            bobRb.mass = bobMass;
            bobs.Add(bob);

            HingeJoint hinge = bob.AddComponent<HingeJoint>();
            hinge.connectedBody = pivotRb;
            hinge.anchor = Vector3.zero;
            hinge.axis = Vector3.forward;

            GameObject rod = new GameObject($"Rod_{i}");
            rod.transform.SetParent(this.transform);
            LineRenderer lr = rod.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = rodMat;
            lr.startColor = rodColor;
            lr.endColor = rodColor;
            lineRenderers.Add(lr);
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < numberOfPendulums; i++)
        {
            if (pivots[i] != null && bobs[i] != null && lineRenderers[i] != null)
            {
                lineRenderers[i].SetPosition(0, pivots[i].transform.position);
                lineRenderers[i].SetPosition(1, bobs[i].transform.position);
            }
        }
    }
}