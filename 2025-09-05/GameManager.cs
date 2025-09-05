
using UnityEngine;

// V3: Restoring domino functionality on a stable base.
public class GameManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("GameManager starting...");

        // 1. Ensure we have a main camera.
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("CRITICAL: No Main Camera found. Please ensure your scene has a camera tagged 'MainCamera'.");
            return;
        }
        // Position the camera for a good view of the dominoes.
        mainCamera.transform.position = new Vector3(0, 15, -20);
        mainCamera.transform.rotation = Quaternion.Euler(45, 0, 0);
        mainCamera.clearFlags = CameraClearFlags.Skybox; // Use default skybox for a nicer look.

        // 2. Ensure we have a light.
        if (FindObjectOfType<Light>() == null)
        {
            Debug.LogWarning("No light found in scene. Creating a default Directional Light.");
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
        }

        // 3. Create the ground plane.
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(10, 1, 10);
        Debug.Log("Ground plane created.");

        // 4. Add the other managers to the scene.
        if (FindObjectOfType<DominoGenerator>() == null)
        {
            new GameObject("DominoGenerator").AddComponent<DominoGenerator>();
            Debug.Log("DominoGenerator object created.");
        }
        if (FindObjectOfType<InputManager>() == null)
        {
            new GameObject("InputManager").AddComponent<InputManager>();
            Debug.Log("InputManager object created.");
        }
    }
}
