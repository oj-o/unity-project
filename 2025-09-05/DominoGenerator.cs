
using UnityEngine;

public class DominoGenerator : MonoBehaviour
{
    public enum LayoutType { Line, Circle }

    [Header("Domino Settings")]
    public Vector3 dominoScale = new Vector3(0.2f, 1.5f, 0.8f);

    [Header("Layout Settings")]
    public LayoutType layout = LayoutType.Circle; // Default to Circle layout for testing
    public int dominoCount = 50;
    public float spacing = 10f; // Use as radius for the Circle layout

    void Start()
    {
        Debug.Log("DominoGenerator: Start() called.");
        GenerateDominos();
    }

    void GenerateDominos()
    {
        Debug.Log($"DominoGenerator: Generating {dominoCount} dominos in a {layout} layout.");
        switch (layout)
        {
            case LayoutType.Line:
                GenerateLine();
                break;
            case LayoutType.Circle:
                GenerateCircle();
                break;
        }
    }

    void GenerateLine()
    {
        // Note: Spacing for line might need to be smaller, e.g., 1.0f
        for (int i = 0; i < dominoCount; i++)
        {
            Vector3 position = new Vector3(i * 1.0f, dominoScale.y / 2, 0);
            CreateDomino(position, Quaternion.identity, $"Domino_" + i);
        }
    }

    void GenerateCircle()
    {
        for (int i = 0; i < dominoCount; i++)
        {
            float angle = i * (360f / dominoCount);
            float radian = angle * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Sin(radian) * spacing, dominoScale.y / 2, Mathf.Cos(radian) * spacing);
            Quaternion rotation = Quaternion.Euler(0, -angle, 0);
            CreateDomino(position, rotation, $"Domino_" + i);
        }
    }

    void CreateDomino(Vector3 position, Quaternion rotation, string name)
    {
        GameObject domino = GameObject.CreatePrimitive(PrimitiveType.Cube);
        domino.name = name;
        domino.tag = "Domino";
        domino.transform.localScale = dominoScale;
        domino.transform.position = position;
        domino.transform.rotation = rotation;
        domino.transform.SetParent(transform); // Set parent to the generator object

        // Add physics
        domino.AddComponent<Rigidbody>();
        Debug.Log($"Created domino: {name} at position {position}");
    }
}
