using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class PlaceDomino : MonoBehaviour
{
    [Header("Required Components")]
    public SplineContainer spline;      // The spline path to follow
    public GameObject dominoPrefab;     // The prefab for the domino to be placed
    public GameObject starterPrefab;    // The prefab for the starter domino

    [Header("Settings")]
    public float donimoDistance = 1.2f; // Increased distance for stability
    public float starterDistance = 0.6f; // Increased distance for stability
    public float dispY = 0.0f; // Vertical displacement for dominoes

    // This method is called when the script instance is being loaded.
    void Start()
    {
        // --- VALIDATION CHECKS ---
        if (spline == null)
        {
            Debug.LogError("Spline Container is not assigned on " + gameObject.name + ". Please assign a spline.", this);
            return; // Stop execution if the spline is missing
        }
        if (dominoPrefab == null)
        {
            Debug.LogError("Domino Prefab is not assigned on " + gameObject.name + ". Please assign a prefab.", this);
            return; // Stop execution if the domino prefab is missing
        }
        if (starterPrefab == null)
        {
            Debug.LogWarning("Starter Prefab is not assigned on " + gameObject.name + ". A regular domino will be used instead.", this);
            starterPrefab = dominoPrefab; // Use the regular domino as a fallback
        }

        // If all checks pass, generate the dominoes.
        GenerateDominoes();
    }

    public void GenerateDominoes()
    {
        Debug.Log("Generating dominoes along the spline...");
        // place domino object along the spline
        float gap = starterDistance;

        for (float p = 0; p < spline.Spline.GetLength(); p += gap)
        {
            float3 position;
            float3 upVector;
            float3 tangent;

            // Get the position and rotation of the spline at parameter t
            float t = p / spline.Spline.GetLength();
            spline.Evaluate(t, out position, out tangent, out upVector);
            
            // Calculate the correct Y position for the domino's base to be on the ground (y=0)
            position.y = dominoPrefab.transform.localScale.y / 2f + dispY; 

            // convert tangent to a y-rotation
            float angle = Mathf.Atan2(tangent.x, tangent.z) * Mathf.Rad2Deg;

            if (p == 0)
            {
                position.y = dominoPrefab.transform.localScale.y / 2f + dispY + 0.2f; // Slightly raise the starter domino

                // Instantiate the starter prefab at the calculated position and rotation
                GameObject starter = Instantiate(starterPrefab, (Vector3)position, Quaternion.Euler(0, angle, 0));
                starter.transform.parent = this.transform;
                // Ensure BoxCollider exists
                if (starter.GetComponent<BoxCollider>() == null) { starter.AddComponent<BoxCollider>(); }
            }
            else
            {
                // Instantiate the domino prefab at the calculated position and rotation
                GameObject domino = Instantiate(dominoPrefab, (Vector3)position, Quaternion.Euler(0, angle, 0));
                domino.transform.parent = this.transform;
                // Ensure BoxCollider exists
                if (domino.GetComponent<BoxCollider>() == null) { domino.AddComponent<BoxCollider>(); }
            }
            
            // After the first domino (starter), switch to the regular domino distance.
            if (p == 0) 
            {
                gap = donimoDistance;
            }
        }
        Debug.Log("Domino generation complete.");
    }
}
