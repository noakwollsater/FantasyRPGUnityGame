using Unity.FantasyKingdom;
using UnityEngine;
using UnityEngine.UI;

public class ClassIconSelector : MonoBehaviour
{
    [SerializeField] private Sprite[] classIcons; // Array of class icons
    [SerializeField] private Image SkillTreeIcon; // The icon to be displayed

    public string ClassName; // The name of the class

    private LoadCharacterData _loadCharacterData;

    private void Awake()
    {
        GameObject HUD = GameObject.FindGameObjectWithTag("HUD");
        if (HUD != null)
        {
            _loadCharacterData = HUD.GetComponent<LoadCharacterData>();
        }
        else
        {
            Debug.LogError("HUD not found");
        }
    }

    private void OnEnable()
    {
        ClassName = _loadCharacterData.className;
        foreach (var icon in classIcons)
        {
            if (icon.name == ClassName)
            {
                SkillTreeIcon.sprite = icon;
                break;
            }
        }
    }
}
