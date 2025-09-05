using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelection : MonoBehaviour
{
    void Awake()
    {
        SetupCamera();
        CreateModeSelectionUI();
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

    void CreateModeSelectionUI()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        CreateText("TitleText", "Select Game Mode", new Vector2(0, 150), canvas.transform, 36, Color.white);

        CreateButton("PendulumGameButton", "1. Mouse Control Game", new Vector2(0, 50), canvas.transform, "PendulumGame");
        CreateButton("PendulumSim3DButton", "2. 3D Pendulum Simulation", new Vector2(0, 0), canvas.transform, "PendulumSim3D");
        CreateButton("PendulumSim2DMultiButton", "3. 2D Multi Pendulum Sim", new Vector2(0, -50), canvas.transform, "PendulumSim2DMulti");
    }

    Text CreateText(string name, string content, Vector2 anchoredPos, Transform parent, int fontSize, Color color)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 60);
        rect.anchoredPosition = anchoredPos;
        return text;
    }

    Button CreateButton(string name, string buttonText, Vector2 anchoredPos, Transform parent, string sceneToLoad)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        buttonObj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        Button button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => SceneManager.LoadScene(sceneToLoad));

        RectTransform rect = buttonObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 50);
        rect.anchoredPosition = anchoredPos;

        CreateText("ButtonText", buttonText, Vector2.zero, buttonObj.transform, 28, Color.white);

        return button;
    }
}