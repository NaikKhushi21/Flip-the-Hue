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