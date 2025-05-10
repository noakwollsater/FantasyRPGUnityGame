using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.FantasyKingdom;

public class BodyCustomizationTriangle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RectTransform triangleUI;  // Assign Triangle UI Image
    public RectTransform selector;    // Assign Draggable Selector
    public CharacterCreation characterCreation; // Reference to Character System
    public BlendShapeChanger BlendShapeChanger; // Reference to Blend Shape Changer

    public Vector2 A, B, C; // Triangle corners in UI space

    void Start()
    {
        // Get World-Space Positions of Triangle Corners (Assuming an Equilateral Triangle)
        float width = triangleUI.rect.width;
        float height = triangleUI.rect.height;

        A = new Vector2(0, height / 2);    // Top (Muscle)
        B = new Vector2(-width / 2, -height / 2);  // Left (Skinny)
        C = new Vector2(width / 2, -height / 2);   // Right (Fat)

        // Validate Triangle
        if (!IsValidTriangle(A, B, C))
        {
            Debug.LogError("Invalid Triangle Configuration: Points are collinear.");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convert screen position to local UI position
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(triangleUI, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            return;
        }

        // Check if the local point is within the triangle UI bounds
        if (!IsPointInsidePanel(localPoint))
        {
            return;
        }

        // Keep within Triangle Bounds
        localPoint = ClampPointToTriangle(localPoint);

        // Update Selector Position
        selector.localPosition = localPoint;

        // Convert to Barycentric Coordinates for Blend Values
        Vector3 bary = Barycentric(localPoint, A, B, C);

        // Convert to 0-100 range for character system
        float muscle = bary.x * 100;
        float skinny = bary.y * 100;
        float fat = bary.z * 100;

        // Apply blend shape changes
        characterCreation._dictionaryLibrary.BodySizeSkinnyBlendValue = skinny;
        characterCreation._dictionaryLibrary.BodySizeHeavyBlendValue = fat;
        characterCreation._dictionaryLibrary.MusclesBlendValue = muscle;

        // Update character attributes based on body composition
        BlendShapeChanger.UpdateBodyComposition();
    }

    private bool IsPointInsidePanel(Vector2 localPoint)
    {
        // Get panel dimensions
        float width = triangleUI.rect.width / 2f;
        float height = triangleUI.rect.height / 2f;

        // Check if point is within the panel bounds
        return (localPoint.x >= -width && localPoint.x <= width &&
                localPoint.y >= -height && localPoint.y <= height);
    }

    public void OnBeginDrag(PointerEventData eventData) { }
    public void OnEndDrag(PointerEventData eventData) { }

    // Function to check if the points form a valid triangle
    private bool IsValidTriangle(Vector2 a, Vector2 b, Vector2 c)
    {
        float area = Mathf.Abs((b.x - a.x) * (c.y - a.y) - (c.x - a.x) * (b.y - a.y)) / 2f;
        return area > 0.0001f; // Ensure non-zero area
    }

    // Function to keep the selector inside the triangle
    private Vector2 ClampPointToTriangle(Vector2 p)
    {
        // Compute Barycentric coordinates
        Vector3 bary = Barycentric(p, A, B, C);

        // If outside the triangle, clamp the barycentric coordinates correctly
        if (bary.x < 0) bary.x = 0;
        if (bary.y < 0) bary.y = 0;
        if (bary.z < 0) bary.z = 0;

        // Normalize so that they still sum to 1
        float sum = bary.x + bary.y + bary.z;
        if (sum > 0)
        {
            bary /= sum;
        }

        // Convert back to Cartesian coordinates
        return bary.x * A + bary.y * B + bary.z * C;
    }

    // Convert local point to Barycentric coordinates
    private Vector3 Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float detT = (b.y - c.y) * (a.x - c.x) + (c.x - b.x) * (a.y - c.y);
        float alpha = ((b.y - c.y) * (p.x - c.x) + (c.x - b.x) * (p.y - c.y)) / detT;
        float beta = ((c.y - a.y) * (p.x - c.x) + (a.x - c.x) * (p.y - c.y)) / detT;
        float gamma = 1 - alpha - beta;
        return new Vector3(alpha, beta, gamma);
    }

    // Debugging visualization in the Unity Editor
    void OnDrawGizmos()
    {
        if (triangleUI == null) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(triangleUI.TransformPoint(A), 5f);
        Gizmos.DrawSphere(triangleUI.TransformPoint(B), 5f);
        Gizmos.DrawSphere(triangleUI.TransformPoint(C), 5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(triangleUI.TransformPoint(A), triangleUI.TransformPoint(B));
        Gizmos.DrawLine(triangleUI.TransformPoint(B), triangleUI.TransformPoint(C));
        Gizmos.DrawLine(triangleUI.TransformPoint(C), triangleUI.TransformPoint(A));
    }
}
