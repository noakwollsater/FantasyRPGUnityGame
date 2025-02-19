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
    public Dictionary<string, Dictionary<string, Color>> speciesColors = new Dictionary<string, Dictionary<string, Color>>
{
    { "Human", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.96f, 0.80f, 0.69f) },
            { "Eyes", new Color(1.0f, 1.0f, 1.0f) },
            { "Hair", new Color(0.50f, 0.35f, 0.20f) },
            { "Fingernails", new Color(0.85f, 0.65f, 0.545f) } 
        }
    },
    { "Goblin", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.533f, 0.510f, 0.298f) },
            { "Eyes", new Color(0.85f, 0.10f, 0.10f) },
            { "Hair", new Color(0.10f, 0.10f, 0.10f) },
            { "Fingernails", new Color(0.282f, 0.266f, 0.165f) }

        }
    },
    { "Orc", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.30f, 0.40f, 0.20f) },
            { "Eyes", new Color(0.90f, 0.80f, 0.50f) }, 
            { "Hair", new Color(0.10f, 0.10f, 0.10f) } 
        }
    },
    { "Elf", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.90f, 0.85f, 0.75f) },
            { "Eyes", new Color(1.0f, 1.0f, 1.0f) },
            { "Hair", new Color(1.0f, 1.0f, 0.85f) }   
        }
    },
    { "Dwarf", new Dictionary<string, Color>
        {
            { "Skin", new Color(0.85f, 0.70f, 0.60f) },
            { "Eyes", new Color(1.0f, 1.0f, 1.0f) },
            { "Hair", new Color(0.70f, 0.40f, 0.20f) }  
        }
    }
};
    public Dictionary<string, List<string>> bodyPartMappings = new Dictionary<string, List<string>>
    {
        { "Skin", new List<string> { "skin", "nose", "ear", "eyelids", "fingernails" } },
        { "Eyes", new List<string> { "eye outer" } },
        { "Hair", new List<string> { "hair", "facial hair", "eyebrow" } },
            { "Fingernails", new List<string> { "fingernail" } }
    };

}