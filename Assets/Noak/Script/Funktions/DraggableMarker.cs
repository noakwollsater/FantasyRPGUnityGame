using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableMarker : MonoBehaviour, IDragHandler
{
    public RectTransform triangleArea;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(triangleArea, eventData.position, eventData.pressEventCamera, out localPos);

        // Clamp the position within the triangle bounds
        localPos.x = Mathf.Clamp(localPos.x, 0, triangleArea.sizeDelta.x);
        localPos.y = Mathf.Clamp(localPos.y, 0, triangleArea.sizeDelta.y);
        transform.localPosition = localPos;
    }
}
