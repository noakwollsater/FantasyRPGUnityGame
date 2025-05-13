using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class SkillData
{
    public string name;
    public string description;
    public Sprite icon; // You assign this in Inspector
}

public class ClassSkills : MonoBehaviour
{
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private SkillData[] allSkills;

    [SerializeField] private TMP_Text skillName;
    [SerializeField] private TMP_Text description;

    [SerializeField] private Button skill1;
    [SerializeField] private Button skill2;
    [SerializeField] private Button skill3;

    [SerializeField] private Image skill1Image;
    [SerializeField] private Image skill2Image;
    [SerializeField] private Image skill3Image;

    private string defaultSkillText;
    private string defaultDescriptionText;

    private string lastSelectedClass;

    private void OnEnable()
    {
        string selectedClass = ClassSelectionUI.SelectedClass;
        SetSkills(selectedClass);
    }

    private void Update()
    {
        string currentSelectedClass = ClassSelectionUI.SelectedClass;

        if (currentSelectedClass != lastSelectedClass)
        {
            lastSelectedClass = currentSelectedClass;
            SetSkills(currentSelectedClass);
        }
    }


    private void SetSkills(string selectedClass)
    {
        foreach (var classItem in classDatabase.Classes)
        {
            if (classItem.ClassName == selectedClass)
            {
                string[] abilities = classItem.UniqueAbilities;

                if (abilities.Length >= 3)
                {
                    SetupButton(skill1, skill1Image, abilities[0]);
                    SetupButton(skill2, skill2Image, abilities[1]);
                    SetupButton(skill3, skill3Image, abilities[2]);
                }

                // 👇 Set these to the class info, not the previous UI text!
                defaultSkillText = classItem.ClassName;
                defaultDescriptionText = classItem.Description;

                // 👇 Actually update the UI so it's visible immediately
                skillName.text = defaultSkillText;
                description.text = defaultDescriptionText;
                return;
            }
        }
    }

    private void SetupButton(Button button, Image iconTarget, string abilityName)
    {
        SkillData skill = System.Array.Find(allSkills, s => s.name == abilityName);
        if (skill != null)
        {
            button.GetComponent<Image>().sprite = skill.icon;
            iconTarget.sprite = skill.icon;
            EventTrigger trigger = button.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = button.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            AddTrigger(trigger, EventTriggerType.PointerEnter, () =>
            {
                skillName.text = skill.name;
                description.text = skill.description;
            });

            AddTrigger(trigger, EventTriggerType.PointerExit, () =>
            {
                skillName.text = defaultSkillText;
                description.text = defaultDescriptionText;
            });
        }
        else
        {
            Debug.LogWarning($"⚠️ Skill '{abilityName}' not found in allSkills!");
        }
    }

    private void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((_) => action());
        trigger.triggers.Add(entry);
    }
}