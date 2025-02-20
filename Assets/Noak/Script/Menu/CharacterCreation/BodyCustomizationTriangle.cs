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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(triangleUI, eventData.position, eventData.pressEventCamera, out localPoint);

        // Keep within Triangle Bounds
        localPoint = ClampPointToTriangle(localPoint);

        // Update Selector Position
        selector.localPosition = localPoint;

        // Convert to Barycentric Coordinates for Blend Values
        Vector3 bary = Barycentric(localPoint, A, B, C);

        // Apply values to character customization
        characterCreation._dictionaryLibrary.BodySizeSkinnyBlendValue = bary.y * 100;
        characterCreation._dictionaryLibrary.BodySizeHeavyBlendValue = bary.z * 100;
        characterCreation._dictionaryLibrary.MusclesBlendValue = bary.x * 100;

        // Update Character Model
        BlendShapeChanger.UpdateBodyComposition();
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
        float w1 = ((B.y - C.y) * (p.x - C.x) + (C.x - B.x) * (p.y - C.y)) /
                   ((B.y - C.y) * (A.x - C.x) + (C.x - B.x) * (A.y - C.y));

        float w2 = ((C.y - A.y) * (p.x - C.x) + (A.x - C.x) * (p.y - C.y)) /
                   ((B.y - C.y) * (A.x - C.x) + (C.x - B.x) * (A.y - C.y));

        float w3 = 1 - w1 - w2;

        w1 = Mathf.Clamp01(w1);
        w2 = Mathf.Clamp01(w2);
        w3 = Mathf.Clamp01(w3);

        return w1 * A + w2 * B + w3 * C;
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
