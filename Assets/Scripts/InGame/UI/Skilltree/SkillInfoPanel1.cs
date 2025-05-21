using UnityEngine;
using UnityEngine.UI;

public class SkillInfoPanel : MonoBehaviour
{
    public RectTransform panelTransform;
    public Vector2 offset = new Vector2(30f, -30f);
    public Canvas canvas;

    private bool followMouse = false;

    void Update()
    {
        if (followMouse && panelTransform.gameObject.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            panelTransform.position = mousePos + (Vector3)offset;

            ClampToScreen();
        }
    }

    public void ShowAtMouse()
    {
        followMouse = true;
        panelTransform.gameObject.SetActive(true);
    }

    public void ShowAtObject(RectTransform target)
    {
        followMouse = false;
        Vector3 worldPos = target.position;
        panelTransform.position = worldPos + new Vector3(100f, 0f, 0f); // justering höger om target
        panelTransform.gameObject.SetActive(true);
    }

    public void Hide()
    {
        followMouse = false;
        panelTransform.gameObject.SetActive(false);
    }

    private void ClampToScreen()
    {
        Vector3 pos = panelTransform.position;

        float width = panelTransform.rect.width * canvas.scaleFactor;
        float height = panelTransform.rect.height * canvas.scaleFactor;

        pos.x = Mathf.Clamp(pos.x, 0 + width / 2f, Screen.width - width / 2f);
        pos.y = Mathf.Clamp(pos.y, 0 + height / 2f, Screen.height - height / 2f);

        panelTransform.position = pos;
    }
}
