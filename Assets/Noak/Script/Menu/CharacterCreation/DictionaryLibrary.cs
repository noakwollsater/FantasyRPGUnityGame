using UnityEngine;
using System.Collections;
using Synty.SidekickCharacters.Enums;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewDictionaryLibrary", menuName = "Custom/Dictionary Library")]
public class DictionaryLibrary : ScriptableObject
{
    public Dictionary<CharacterPartType, int> _partIndexDictionary = new Dictionary<CharacterPartType, int>();
    public Dictionary<CharacterPartType, Dictionary<string, string>> _availablePartDictionary = new Dictionary<CharacterPartType, Dictionary<string, string>>();
    public Dictionary<CharacterPartType, Dictionary<string, string>> _partLibrary;
    public Dictionary<string, Color> speciesSkinColors = new Dictionary<string, Color>
{
    { "Human", new Color(0.96f, 0.80f, 0.69f) },  // Light skin tone
    { "Goblin", new Color(0.20f, 0.50f, 0.20f) }, // Greenish goblin tone
    { "Orc", new Color(0.30f, 0.40f, 0.20f) },   // Darker green
    { "Elf", new Color(0.90f, 0.85f, 0.75f) },   // Pale
    { "Dwarf", new Color(0.85f, 0.70f, 0.60f) }, // Slightly tanned
    { "Darkelf", new Color(0.25f, 0.25f, 0.35f) }, // Greyish
    { "Draknoit", new Color(0.45f, 0.30f, 0.25f) }, // Slightly reptilian
    { "Lynx", new Color(0.75f, 0.55f, 0.35f) }, // Furry tone
    { "Hobbit", new Color(0.92f, 0.78f, 0.67f) }, // Similar to human
    { "Ape", new Color(0.40f, 0.30f, 0.25f) }   // Dark fur/skin tone
};

}