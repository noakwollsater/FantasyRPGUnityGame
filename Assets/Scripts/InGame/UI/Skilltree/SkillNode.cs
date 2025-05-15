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
    [Header("Skill Tree Cost")]
    public int depthFromStart = 0; // Sätt manuellt i Editorn
    public TMP_Text costText; // (valfri UI-display)

    public int GetSkillCost()
    {
        if (depthFromStart == int.MaxValue)
        {
            Debug.LogWarning($"{name} has unset depthFromStart! Defaulting cost to 999.");
            return 999; // placeholder för ogiltig nod
        }

        return 1 + (depthFromStart * 2);
    }

    private void OnEnable()
    {
        if (IsUnlocked)
        {
            ForceSelectedVisualState();
        }
    }

    private void Start()
    {
        skillButton.onClick.AddListener(ActivateSkill);

        if (isStartingSkill)
        {
            SetInteractable(true);
        }
        else
        {
            SetInteractable(false);
        }

        foreach (var conn in connections)
        {
            if (conn.sliderToNext != null)
                conn.sliderToNext.value = 0;

            if (conn.nextSkill != null)
                conn.nextSkill.SetInteractable(false);

            Debug.Log($"{name} connection to → {conn.nextSkill?.name ?? "NULL"}");
        }

        var loader = FindObjectOfType<LoadCharacterData>();
        if (loader != null)
        {
            string currentTree = GetComponentInParent<SkillTree>().name;

            if (loader.unlockedSkillTrees.TryGetValue(currentTree, out var unlockedSkills))
            {
                if (unlockedSkills.Contains(skillName))
                {
                    IsUnlocked = true;
                    ForceSelectedVisualState();
                    return;
                }
            }
        }
    }

    private void Update()
    {
        // Valfritt: visa poängkostnad
        if (!IsUnlocked && costText != null)
        {
            costText.text = $"Kostar: {GetSkillCost()}";
        }
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


    public void ReceiveConnection()
    {
        fulfilledConnections++;

        if (fulfilledConnections >= requiredConnectionsToUnlock && !IsUnlocked)
        {
            SetInteractable(true);
        }
    }

    public void SetInteractable(bool state)
    {
        if (IsUnlocked) return;
        skillButton.interactable = state;
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
        var loader = FindObjectOfType<LoadCharacterData>();
        if (loader != null)
        {
            string currentTree = GetComponentInParent<SkillTree>().skillTreeName;

            if (!loader.unlockedSkillTrees.ContainsKey(currentTree))
                loader.unlockedSkillTrees[currentTree] = new List<string>();

            if (!loader.unlockedSkillTrees[currentTree].Contains(skillName))
                loader.unlockedSkillTrees[currentTree].Add(skillName);
        }

        var btn = skillButton;
        var animator = btn.animator;
        if (animator != null)
        {
            animator.ResetTrigger(btn.animationTriggers.normalTrigger);
            animator.ResetTrigger(btn.animationTriggers.highlightedTrigger);
            animator.ResetTrigger(btn.animationTriggers.pressedTrigger);
            animator.ResetTrigger(btn.animationTriggers.disabledTrigger);
            animator.SetTrigger(btn.animationTriggers.selectedTrigger);
        }

        btn.transition = Selectable.Transition.None;
        btn.interactable = false;

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
