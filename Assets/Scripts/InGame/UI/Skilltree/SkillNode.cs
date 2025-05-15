using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using Unity.FantasyKingdom;

public class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button skillButton;
    public List<SkillConnection> connections;

    [Header("Skill Info")]
    public string skillName;
    [TextArea] public string skillDescription;

    [Header("Setup")]
    public bool isStartingSkill = false;

    public bool IsUnlocked { get; private set; }

    [Header("Advanced Unlock Conditions")]
    public int requiredConnectionsToUnlock = 1;
    private int fulfilledConnections = 0;

    [Header("Skill Tree Cost")]
    public int depthFromStart = 0;
    public TMP_Text costText;

    private void OnEnable()
    {
        if (IsUnlocked)
            ForceSelectedVisualState();
    }

    private void Start()
    {
        skillButton.onClick.AddListener(ActivateSkill);

        if (isStartingSkill)
            SetInteractable(true);
        else
            SetInteractable(false);

        // Återställ sliders och lås nästa skills
        foreach (var conn in connections)
        {
            if (conn.sliderToNext != null)
                conn.sliderToNext.value = 0;

            if (conn.nextSkill != null)
                conn.nextSkill.SetInteractable(false);
        }

        LoadSavedUnlockState();
    }

    private void Update()
    {
        if (!IsUnlocked && costText != null)
        {
            costText.text = $"Kostar: {GetSkillCost()}";
        }
    }

    public int GetSkillCost()
    {
        return 1 + (depthFromStart * 2);
    }

    public void SetInteractable(bool state)
    {
        if (IsUnlocked) return;
        skillButton.interactable = state;
    }


    public void ReceiveConnection()
    {
        fulfilledConnections++;

        if (fulfilledConnections >= requiredConnectionsToUnlock && !IsUnlocked)
        {
            SetInteractable(true);
        }
    }

    public void ActivateSkill()
    {
        if (IsUnlocked) return;

        int cost = GetSkillCost();
        if (!SkillManager.Instance.HasEnoughPoints(cost))
        {
            Debug.Log("❌ Not enough level points!");
            return;
        }

        SkillManager.Instance.SpendPoints(cost);
        IsUnlocked = true;

        SaveSkillToCharacter();

        ForceSelectedVisualState();

        foreach (var conn in connections)
        {
            StartCoroutine(AnimateSlider(conn));
        }

        GetComponentInParent<SkillTree>().CheckTreeCompletion();
    }

    private IEnumerator AnimateSlider(SkillConnection conn)
    {
        if (conn.sliderToNext == null || conn.nextSkill == null)
            yield break;

        float duration = 0.5f;
        float elapsed = 0f;
        float start = conn.sliderToNext.value;

        while (elapsed < duration)
        {
            conn.sliderToNext.value = Mathf.Lerp(start, 1f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        conn.sliderToNext.value = 1f;
        conn.nextSkill.ReceiveConnection();
    }

    private void ForceSelectedVisualState()
    {
        var btn = skillButton;
        var animator = btn.animator;
        if (animator != null)
        {
            animator.ResetTrigger(btn.animationTriggers.normalTrigger);
            animator.ResetTrigger(btn.animationTriggers.highlightedTrigger);
            animator.ResetTrigger(btn.animationTriggers.disabledTrigger);
            animator.SetTrigger(btn.animationTriggers.selectedTrigger);
        }

        btn.transition = Selectable.Transition.None;
        btn.interactable = false;
    }

    private void LoadSavedUnlockState()
    {
        var loader = FindObjectOfType<LoadCharacterData>();
        if (loader == null)
        {
            Debug.LogWarning("⚠️ LoadCharacterData not found in scene.");
            return;
        }

        string currentTree = GetComponentInParent<SkillTree>().skillTreeName;

        if (!loader.unlockedSkillTrees.TryGetValue(currentTree, out var unlockedSkills))
            return;

        // ✅ Exakt match
        if (unlockedSkills.Contains(skillName))
        {
            ApplyUnlockedVisuals();
            return;
        }

        // 🔍 Kolla om någon högre nivå av denna skill är upplåst (t.ex. "Jump 2" → "Jump 1" ska highlightas)
        string prefix = skillName.Contains(" ") ? skillName.Split(' ')[0] : skillName;
        int thisLevel = TryGetSkillLevel(skillName);

        foreach (var unlocked in unlockedSkills)
        {
            if (unlocked.StartsWith(prefix + " "))
            {
                int unlockedLevel = TryGetSkillLevel(unlocked);

                if (unlockedLevel > thisLevel)
                {
                    ApplyUnlockedVisuals();
                    return;
                }
            }
        }
    }
    private int TryGetSkillLevel(string name)
    {
        string[] parts = name.Split(' ');
        if (parts.Length < 2) return 0;

        return int.TryParse(parts[1], out int level) ? level : 0;
    }
    private void ApplyUnlockedVisuals()
    {
        IsUnlocked = true;
        ForceSelectedVisualState();

        foreach (var conn in connections)
        {
            if (conn.sliderToNext != null)
                conn.sliderToNext.value = 1f;
        }

        StartCoroutine(DelayedSendConnections());
    }


    private IEnumerator DelayedSendConnections()
    {
        yield return null; // Vänta ett frame så att alla skillnodes har kört Start()

        foreach (var conn in connections)
        {
            if (conn.nextSkill != null)
            {
                conn.nextSkill.ReceiveConnection();
            }
        }
    }


    private void SaveSkillToCharacter()
    {
        var loader = FindObjectOfType<LoadCharacterData>();
        if (loader == null) return;

        string currentTree = GetComponentInParent<SkillTree>().skillTreeName;

        if (!loader.unlockedSkillTrees.ContainsKey(currentTree))
            loader.unlockedSkillTrees[currentTree] = new List<string>();

        var list = loader.unlockedSkillTrees[currentTree];

        string prefix = skillName.Contains(" ") ? skillName.Split(' ')[0] : skillName;
        list.RemoveAll(skill => skill.StartsWith(prefix + " ") && skill != skillName);

        if (!list.Contains(skillName))
            list.Add(skillName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsUnlocked)
        {
            SkillInfoPanel.Instance.Show(skillName, skillDescription, GetSkillCost());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillInfoPanel.Instance.Hide();
    }
}
