using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttributeSet
{
    public int Strength;
    public int Dexterity;
    public int Constitution;
    public int Intelligence;
    public int Wisdom;
    public int Charisma;

    // ✅ Add this constructor
    public AttributeSet(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma)
    {
        Strength = strength;
        Dexterity = dexterity;
        Constitution = constitution;
        Intelligence = intelligence;
        Wisdom = wisdom;
        Charisma = charisma;
    }

    private static System.Random rng = new System.Random();

    public void ApplyBodyCompositionModifiers(float muscle, float skinny, float fat)
    {
        // Strength heavily benefits from muscle, slightly penalized by thinness
        Strength += Mathf.RoundToInt(muscle * 3);  // Max +3 STR at full muscle
        Strength -= Mathf.RoundToInt(skinny * 1);  // Max -1 STR if very thin

        // Dexterity now penalized more by muscle, and improved by thinness
        Dexterity += Mathf.RoundToInt(skinny * 3); // Max +3 DEX if very thin
        Dexterity -= Mathf.RoundToInt(fat * 2);    // Max -2 DEX if very heavy
        Dexterity -= Mathf.RoundToInt(muscle * 1); // New: Muscle slightly reduces dexterity (-1 max)

        // Constitution benefits from fat, but still weakened by thinness
        Constitution += Mathf.RoundToInt(fat * 3); // Max +3 CON if very heavy
        Constitution -= Mathf.RoundToInt(skinny * 2); // Max -2 CON if very thin

        // Intelligence now suffers more from high muscle
        Intelligence -= Mathf.RoundToInt(muscle * 2f); // Max -2 INT if very muscular (was 1.5)
        Intelligence += Mathf.RoundToInt(fat * 1.5f);  // Max +1.5 INT if very heavy

        // Wisdom now suffers more from muscle
        Wisdom += Mathf.RoundToInt(skinny * 1.5f); // Max +1.5 WIS if very thin
        Wisdom -= Mathf.RoundToInt(fat * 1);       // Slight -1 WIS if very heavy
        Wisdom -= Mathf.RoundToInt(muscle * 1.5f); // New: Max -1.5 WIS if very strong

        // Charisma benefits from strength, but still penalized by fat
        Charisma += Mathf.RoundToInt(muscle * 2);  // Max +2 CHA if very strong (was 1)
        Charisma -= Mathf.RoundToInt(fat * 1);     // Max -1 CHA if very overweight

        // **Stat limits to avoid high/low starting stats**
        int minStat = 5;  // Prevents stats dropping too low
        int maxStat = 15; // Prevents overpowered stats

        Strength = Mathf.Clamp(Strength, minStat, maxStat);
        Dexterity = Mathf.Clamp(Dexterity, minStat, maxStat);
        Constitution = Mathf.Clamp(Constitution, minStat, maxStat);
        Intelligence = Mathf.Clamp(Intelligence, minStat, maxStat);
        Wisdom = Mathf.Clamp(Wisdom, minStat, maxStat);
        Charisma = Mathf.Clamp(Charisma, minStat, maxStat);
    }

    // Rolling 4d6, drop the lowest
    public static int RollStat()
    {
        int[] rolls = { rng.Next(1, 7), rng.Next(1, 7), rng.Next(1, 7), rng.Next(1, 7) };
        Array.Sort(rolls);
        return rolls[1] + rolls[2] + rolls[3]; // Sum top 3
    }

    // Generate random attributes for a player
    public static AttributeSet GenerateRandomAttributes()
    {
        return new AttributeSet(RollStat(), RollStat(), RollStat(), RollStat(), RollStat(), RollStat());
    }

    // Apply racial bonuses
    public void ApplyRacialBonuses(AttributeSet racialBonus)
    {
        Strength += racialBonus.Strength;
        Dexterity += racialBonus.Dexterity;
        Constitution += racialBonus.Constitution;
        Intelligence += racialBonus.Intelligence;
        Wisdom += racialBonus.Wisdom;
        Charisma += racialBonus.Charisma;
    }
}

[System.Serializable]
public class Race
{
    public string RaceName;
    public string Description;
    public AttributeSet BaseAttributes; // Lowered base stats
    public AttributeSet RacialBonuses;

    public Race(string raceName, string description, AttributeSet baseAttributes, AttributeSet racialBonuses)
    {
        RaceName = raceName;
        Description = description;
        BaseAttributes = baseAttributes;
        RacialBonuses = racialBonuses;
    }
}

public class RaceDatabase : MonoBehaviour
{
    public static RaceDatabase Instance;
    public List<Race> Races = new List<Race>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeRaces();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeRaces()
    {
        Races.Add(new Race(
            "Human",
            "Humans are adaptable and resilient, thriving in any environment. They are well-rounded with no major weaknesses or strengths.",
            new AttributeSet(8, 8, 8, 8, 8, 8),
            new AttributeSet(2, 2, 2, 2, 2, 2)
        ));

        Races.Add(new Race(
            "Goblin",
            "Goblins are small, nimble, and cunning. While physically weaker, they compensate with intelligence and agility, excelling in trickery and ambush tactics.",
            new AttributeSet(6, 10, 6, 10, 6, 8),
            new AttributeSet(0, 2, -1, 1, 0, 0)
        ));

        Races.Add(new Race(
            "Orc",
            "Orcs are powerful warriors with immense strength but often lack finesse and intelligence. They thrive in battle and have a strong warrior culture.",
            new AttributeSet(10, 6, 9, 5, 6, 6),
            new AttributeSet(2, -1, 1, -2, 0, 0)
        ));

        Races.Add(new Race(
            "Ape",
            "Apefolk are strong and agile, known for their primal instincts and raw power. Their intelligence is slightly lower, but they have excellent survival skills.",
            new AttributeSet(9, 7, 9, 6, 7, 6),
            new AttributeSet(2, 1, 2, -2, 1, -1)
        ));

        Races.Add(new Race(
            "Dark Elf",
            "Dark Elves, or Drow, are mysterious and cunning, often living underground or in hidden cities. They possess high dexterity and intelligence, but their arrogance can be a weakness.",
            new AttributeSet(7, 10, 7, 9, 9, 8),
            new AttributeSet(-1, 2, 0, 1, 1, 0)
        ));

        Races.Add(new Race(
            "Drakonite",
            "Drakonites are descendants of dragons, boasting incredible strength and resilience. They lack some agility but make up for it with their intimidating presence.",
            new AttributeSet(10, 6, 9, 8, 7, 6),
            new AttributeSet(2, -1, 1, 0, 0, 0)
        ));

        Races.Add(new Race(
            "Dwarf",
            "Dwarves are stout, durable, and skilled craftsmen. They possess strong constitution and wisdom, but their lack of agility makes them less nimble in combat.",
            new AttributeSet(9, 6, 10, 7, 9, 5),
            new AttributeSet(2, -1, 2, 0, 2, -2)
        ));

        Races.Add(new Race(
            "Elf",
            "Elves are elegant and intelligent beings with a deep connection to nature and magic. They are highly agile but lack the physical durability of other races.",
            new AttributeSet(6, 10, 7, 9, 9, 8),
            new AttributeSet(-1, 2, 0, 2, 1, 0)
        ));

        Races.Add(new Race(
            "Hobbit",
            "Hobbits (also known as Shortmen) are small, clever, and charismatic. They may not be the strongest, but their agility and charm make them excellent diplomats and thieves.",
            new AttributeSet(6, 10, 7, 8, 8, 10),
            new AttributeSet(0, 2, 0, 0, 0, 2)
        ));

        Races.Add(new Race(
            "Lynx",
            "Lynxfolk (also known as Catmen) are quick, dexterous, and instinctual hunters. They excel in speed and agility but lack physical endurance.",
            new AttributeSet(7, 12, 6, 9, 8, 8),
            new AttributeSet(0, 2, -1, 1, 0, 0)
        ));

        Debug.Log("Races initialized!");
    }


    public Race GetRace(string raceName)
    {
        return Races.Find(r => r.RaceName == raceName);
    }
}

public class CharacterStatCreation
{
    public static AttributeSet CreateCharacter(string raceName, bool usePointBuy, float muscle, float skinny, float fat)
    {
        Race selectedRace = RaceDatabase.Instance.GetRace(raceName);

        AttributeSet playerAttributes;
        if (usePointBuy)
        {
            playerAttributes = PointBuySystem.GeneratePointBuyAttributes();
        }
        else
        {
            playerAttributes = AttributeSet.GenerateRandomAttributes();
        }

        // Apply racial bonuses
        playerAttributes.ApplyRacialBonuses(selectedRace.RacialBonuses);

        // Apply body composition modifiers
        playerAttributes.ApplyBodyCompositionModifiers(muscle, skinny, fat);

        return playerAttributes;
    }
}

public class PointBuySystem
{
    private const int TOTAL_POINTS = 27;
    private static int[] pointCosts = { 0, 1, 2, 3, 4, 5, 7, 9 }; // Cost for stats 8-15

    public static AttributeSet GeneratePointBuyAttributes()
    {
        int remainingPoints = TOTAL_POINTS;
        int[] stats = { 8, 8, 8, 8, 8, 8 }; // Base stats at 8
        System.Random rng = new System.Random();

        while (remainingPoints > 0)
        {
            int index = rng.Next(0, 6); // Pick a random stat
            if (stats[index] < 15)
            {
                int cost = pointCosts[stats[index] - 8 + 1];
                if (remainingPoints >= cost)
                {
                    stats[index]++;
                    remainingPoints -= cost;
                }
            }
        }

        return new AttributeSet(stats[0], stats[1], stats[2], stats[3], stats[4], stats[5]);
    }
}