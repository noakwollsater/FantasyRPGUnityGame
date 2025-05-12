using System.Collections.Generic;
using UnityEngine;

public class ClassDatabase : MonoBehaviour
{
    public static ClassDatabase Instance;
    public List<Class> Classes = new List<Class>();

    [SerializeField] private GameObject skillPanel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeClasses();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        if (skillPanel != null)
        {
            skillPanel.SetActive(true);
        }
    }
    void OnDisable()
    {
        if (skillPanel != null)
        {
            skillPanel.SetActive(false);
        }
    }

    void InitializeClasses()
    {
        Classes.Add(new Class(
            "Barbarian",
            "Brutal warriors who unleash their rage to crush enemies.",
            "Strength",
            "Tank, Melee Fighter",
            new string[] { "Rage", "Unarmored Defense", "Primal Instinct" },
            new AttributeSet(2, 0, 2, -1, 0, 0) // +2 STR, +2 CON, -1 INT
        ));

        Classes.Add(new Class(
            "Bard",
            "Musical magicians and inspirers who support allies.",
            "Charisma",
            "Support, Mage, Social Expert",
            new string[] { "Bardic Inspiration", "Jack of All Trades", "Magical Versatility" },
            new AttributeSet(0, 0, 0, 0, 0, 2) // +2 CHA
        ));

        Classes.Add(new Class(
            "Cleric",
            "Divine spellcasters and healers with holy powers.",
            "Wisdom",
            "Healer, Tank, Mage",
            new string[] { "Divine Domain", "Turn Undead", "Divine Magic" },
            new AttributeSet(0, 0, 1, 0, 2, 0) // +1 CON, +2 WIS
        ));

        Classes.Add(new Class(
            "Druid",
            "Nature's protectors and shape-shifters.",
            "Wisdom",
            "Mage, Support, Tank (via Wild Shape)",
            new string[] { "Wild Shape", "Nature Magic", "Summon Beasts" },
            new AttributeSet(0, 0, 0, 0, 2, 0) // +2 WIS
        ));

        Classes.Add(new Class(
            "Fighter",
            "Masters of weapons and combat tactics.",
            "Strength or Dexterity",
            "Tank, Damage Dealer",
            new string[] { "Action Surge", "Extra Attack", "Combat Styles" },
            new AttributeSet(2, 1, 1, 0, 0, 0) // +2 STR, +1 DEX, +1 CON
        ));

        Classes.Add(new Class(
            "Monk",
            "Fast, unarmed warriors with supernatural abilities.",
            "Dexterity and Wisdom",
            "Damage Dealer, Scout",
            new string[] { "Ki Points", "Unarmed Strikes", "Evasion" },
            new AttributeSet(0, 2, 0, 0, 2, 0) // +2 DEX, +2 WIS
        ));

        Classes.Add(new Class(
            "Paladin",
            "Holy warriors infused with divine power.",
            "Strength and Charisma",
            "Tank, Damage Dealer, Healer",
            new string[] { "Divine Smite", "Lay on Hands", "Sacred Oath" },
            new AttributeSet(2, 0, 1, 0, 0, 2) // +2 STR, +1 CON, +2 CHA
        ));

        Classes.Add(new Class(
            "Ranger",
            "Survivalists and expert archers.",
            "Dexterity and Wisdom",
            "Damage Dealer, Scout, Nature Mage",
            new string[] { "Favored Enemy", "Spellcasting", "Hunter’s Mark" },
            new AttributeSet(0, 2, 0, 0, 2, 0) // +2 DEX, +2 WIS
        ));

        Classes.Add(new Class(
            "Rogue",
            "Stealthy assassins and master thieves.",
            "Dexterity",
            "Damage Dealer, Scout, Trap Specialist",
            new string[] { "Sneak Attack", "Evasion", "Cunning Action" },
            new AttributeSet(0, 2, 0, 0, 0, 1) // +2 DEX, +1 CHA
        ));

        Classes.Add(new Class(
            "Sorcerer",
            "Born with innate magical abilities.",
            "Charisma",
            "Mage, Damage Dealer",
            new string[] { "Sorcery Points", "Metamagic", "Spontaneous Magic" },
            new AttributeSet(0, 0, 0, 2, 0, 2) // +2 INT, +2 CHA
        ));

        Classes.Add(new Class(
            "Warlock",
            "Spellcasters who draw power from powerful beings.",
            "Charisma",
            "Mage, Damage Dealer, Support",
            new string[] { "Pact Magic", "Eldritch Invocations", "Pacts" },
            new AttributeSet(0, 0, 0, 1, 0, 2) // +1 INT, +2 CHA
        ));

        Classes.Add(new Class(
            "Wizard",
            "Masters of arcane knowledge and spellcasting.",
            "Intelligence",
            "Mage, Damage Dealer, Support",
            new string[] { "Spellbook", "Arcane Tradition", "Ritual Casting" },
            new AttributeSet(0, 0, 0, 3, 0, 0) // +3 INT
        ));

        Debug.Log("Classes initialized!");
    }

    public Class GetClass(string className)
    {
        return Classes.Find(c => c.ClassName == className);
    }
}
