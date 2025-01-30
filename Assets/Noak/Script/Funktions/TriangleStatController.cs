using UnityEngine;
using UnityEngine.UI;

public class TriangleStatController : MonoBehaviour
{
    public RectTransform triangleArea; // The UI triangle area
    public RectTransform marker; // The draggable marker

    [Range(0, 100)] public int muscle;
    [Range(0, 100)] public int fat;
    [Range(0, 100)] public int dexterity;

    private Vector2 topCorner = new Vector2(0.5f, 1f); // Normalized position
    private Vector2 bottomLeftCorner = new Vector2(0f, 0f); // Normalized position
    private Vector2 bottomRightCorner = new Vector2(1f, 0f); // Normalized position

    private void Update()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        // Convert marker's position to normalized triangle space
        Vector2 localPos = marker.anchoredPosition / triangleArea.sizeDelta;

        // Calculate proportions
        float totalArea = TriangleArea(topCorner, bottomLeftCorner, bottomRightCorner);
        float area1 = TriangleArea(localPos, bottomLeftCorner, bottomRightCorner) / totalArea;
        float area2 = TriangleArea(localPos, bottomRightCorner, topCorner) / totalArea;
        float area3 = TriangleArea(localPos, topCorner, bottomLeftCorner) / totalArea;

        // Convert proportions to stats
        muscle = Mathf.RoundToInt(area1 * 100);
        fat = Mathf.RoundToInt(area2 * 100);
        dexterity = Mathf.RoundToInt(area3 * 100);
    }

    private float TriangleArea(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return Mathf.Abs((p1.x * (p2.y - p3.y) + p2.x * (p3.y - p1.y) + p3.x * (p1.y - p2.y)) / 2f);
    }
}
