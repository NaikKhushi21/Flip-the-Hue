/*
using UnityEngine;
using TMPro;  // if using TextMeshPro

public class HelperText : MonoBehaviour
{
    public Transform groundOrObstacle;  // The world position to place the text over (e.g., on the ground or obstacle)
    public TextMeshProUGUI helperText;  // UI Text (or TextMeshPro) to display
    public Vector2 screenOffset = new Vector2(0, -30);  // Offset to move the text down (adjust this as needed)

    void Update()
    {
        // Convert world position of ground or obstacle to screen space position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(groundOrObstacle.position);

        // Apply the offset (e.g., move text downward)
        screenPos += (Vector3)screenOffset;

        // Set the position of the helper text on the canvas using RectTransform
        helperText.rectTransform.position = screenPos;
    }
}
*/


using UnityEngine;
using TMPro;

public class HelperText : MonoBehaviour
{
    public Transform groundOrObstacle;       // The world position to place the text over
    public TextMeshProUGUI helperText;       // UI Text (or TextMeshPro) to display
    public Vector2 screenOffset = new Vector2(0, -30);  // Offset in pixels
    private Canvas canvas;                   // Reference to the Canvas

    void Start()
    {
        // Get the Canvas component
        canvas = helperText.canvas;
    }

    void Update()
    {
        if (canvas == null) return;

        // Convert the world position to screen space
        Vector3 screenPos = Camera.main.WorldToScreenPoint(groundOrObstacle.position);

        // Check if the object is visible by the camera
        if (screenPos.z > 0) // z > 0 means the object is in front of the camera
        {
            // Convert screen position to Canvas space
            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.worldCamera,
                out canvasPos);

            // Apply the offset
            canvasPos += screenOffset;

            // Set the helper text's position
            helperText.rectTransform.anchoredPosition = canvasPos;

            // Ensure the text is active
            helperText.gameObject.SetActive(true);
        }
        else
        {
            // Hide the text if the object is not visible
            helperText.gameObject.SetActive(false);
        }
    }
}
