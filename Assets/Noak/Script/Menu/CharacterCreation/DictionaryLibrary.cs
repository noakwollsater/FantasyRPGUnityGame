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
}