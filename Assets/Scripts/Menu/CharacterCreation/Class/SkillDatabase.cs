using System.Collections.Generic;
using UnityEngine;

public class SkillDatabase : MonoBehaviour
{
    public static SkillDatabase Instance;

    public List<SkillData> Skills = new List<SkillData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeSkills();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSkills()
    {
        Skills.Clear();

        AddSkill("Rage", "Enter a battle fury, increasing damage and resistance.");
        AddSkill("Unarmored Defense", "Gain defense bonuses when not wearing armor.");
        AddSkill("Primal Instinct", "Tap into beast-like senses and reflexes.");

        AddSkill("Bardic Inspiration", "Inspire allies to enhance their abilities.");
        AddSkill("Jack of All Trades", "Gain bonuses to all ability checks.");
        AddSkill("Magical Versatility", "Adapt spells to new situations.");

        AddSkill("Divine Domain", "Gain powers from your chosen divine focus.");
        AddSkill("Turn Undead", "Repel undead using divine power.");
        AddSkill("Divine Magic", "Cast healing and protective spells.");

        AddSkill("Wild Shape", "Transform into animals and creatures.");
        AddSkill("Nature Magic", "Cast spells drawn from nature.");
        AddSkill("Summon Beasts", "Call creatures to aid in battle.");

        AddSkill("Action Surge", "Take an extra action this turn.");
        AddSkill("Extra Attack", "Attack more than once per action.");
        AddSkill("Combat Styles", "Gain bonuses from your fighting style.");

        AddSkill("Ki Points", "Use inner energy to perform supernatural abilities.");
        AddSkill("Unarmed Strikes", "Deal damage with martial arts.");
        AddSkill("Evasion", "Dodge area effects with agility.");

        AddSkill("Divine Smite", "Smite enemies with holy energy.");
        AddSkill("Lay on Hands", "Heal wounds by touch.");
        AddSkill("Sacred Oath", "Swear a holy oath granting unique powers.");

        AddSkill("Favored Enemy", "Gain bonuses against a chosen enemy type.");
        AddSkill("Spellcasting", "Use learned or innate spells.");
        AddSkill("Hunter’s Mark", "Track and deal bonus damage to targets.");

        AddSkill("Sneak Attack", "Deal bonus damage when you have advantage.");
        AddSkill("Cunning Action", "Dash, Disengage, or Hide as a bonus action.");

        AddSkill("Sorcery Points", "Fuel magical effects with innate energy.");
        AddSkill("Metamagic", "Alter spells for flexibility.");
        AddSkill("Spontaneous Magic", "Cast spells without preparation.");

        AddSkill("Pact Magic", "Draw magic from a powerful patron.");
        AddSkill("Eldritch Invocations", "Customize spells with special powers.");
        AddSkill("Pacts", "Choose a pact that shapes your abilities.");

        AddSkill("Spellbook", "Record and prepare a wide range of spells.");
        AddSkill("Arcane Tradition", "Specialize in a magical school.");
        AddSkill("Ritual Casting", "Cast spells without using slots.");
    }

    private void AddSkill(string name, string description)
    {
        SkillData skill = new SkillData
        {
            name = name,
            description = description,
            icon = null // You assign this in the Inspector later
        };

        Skills.Add(skill);
    }

    public SkillData GetSkill(string name)
    {
        return Skills.Find(s => s.name == name);
    }
}
