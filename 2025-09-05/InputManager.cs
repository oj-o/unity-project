
using UnityEngine;
using UnityEngine.InputSystem; // Import the new Input System namespace

public class InputManager : MonoBehaviour
{
    public float pushForce = 100f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check for mouse presence and left button click using the new Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
    }

    void HandleMouseClick()
    {
        // Use the new Input System to get the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the clicked object is a domino
            if (hit.collider.CompareTag("Domino"))
            {
                PushDomino(hit.collider.gameObject, hit.point);
            }
        }
    }

    void PushDomino(GameObject domino, Vector3 hitPoint)
    {
        Rigidbody rb = domino.GetComponent<Rigidbody>();
        if (rb != null)
        {   
            // By default, push the domino along its forward (thin) direction
            // This creates a consistent chain reaction along the path
            Vector3 pushDirection = domino.transform.forward;
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }
}
