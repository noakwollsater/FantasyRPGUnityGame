using UnityEngine;
using TMPro;

public class SkillInfoPanel : MonoBehaviour
{
    public static SkillInfoPanel Instance;

    public GameObject panelRoot;
    public RectTransform panelRect;
    public TMP_Text skillNameText;
    public TMP_Text skillDescriptionText;
    public TMP_Text skillCostText;

    public Vector2 offset = new Vector2(30f, -30f);
    public Canvas canvas;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string skillName, string description, int cost)
    {
        skillNameText.text = skillName;
        skillDescriptionText.text = description;
        skillCostText.text = $"{cost} Points";

        PositionPanel(); // Lägg till detta
        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }

    private void PositionPanel()
    {
        Vector2 mousePos = Input.mousePosition;
        panelRect.position = mousePos + (Vector2)offset;

        ClampToScreen();
    }

    private void ClampToScreen()
    {
        Vector3 pos = panelRect.position;

        float width = panelRect.rect.width * canvas.scaleFactor;
        float height = panelRect.rect.height * canvas.scaleFactor;

        pos.x = Mathf.Clamp(pos.x, width / 2f, Screen.width - width / 2f);
        pos.y = Mathf.Clamp(pos.y, height / 2f, Screen.height - height / 2f);

        panelRect.position = pos;
    }
}
