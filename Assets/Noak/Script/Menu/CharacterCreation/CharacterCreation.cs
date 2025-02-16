using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using Synty.SidekickCharacters.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class CharacterCreation : MonoBehaviour
{
    private readonly string _OUTPUT_MODEL_NAME = "Sidekick Character";

    Dictionary<CharacterPartType, int> _partIndexDictionary = new Dictionary<CharacterPartType, int>();
    Dictionary<CharacterPartType, Dictionary<string, string>> _availablePartDictionary = new Dictionary<CharacterPartType, Dictionary<string, string>>();
    private Dictionary<CharacterPartType, Dictionary<string, string>> _partLibrary;

    private DatabaseManager _dbManager;
    private SidekickRuntime _sidekickRuntime;

    [Range(0f, 100f)] public float BodySizeSkinnyBlendValue;
    [Range(0f, 100f)] public float BodySizeHeavyBlendValue;
    [Range(0f, 100f)] public float MusclesBlendValue;

    // For this example we are only interested in Upper Body parts, so we filter the list of all parts to only get the ones we want.
    List<CharacterPartType> upperBodyParts = PartGroup.UpperBody.GetPartTypes();
    List<CharacterPartType> lowerBodyParts = PartGroup.LowerBody.GetPartTypes();
    List<CharacterPartType> headParts = PartGroup.Head.GetPartTypes();

    private static readonly HashSet<CharacterPartType> ExcludedParts = new()
    {
        CharacterPartType.AttachmentBack, CharacterPartType.AttachmentElbowLeft, CharacterPartType.AttachmentElbowRight,
        CharacterPartType.AttachmentShoulderLeft, CharacterPartType.AttachmentShoulderRight, CharacterPartType.AttachmentHipsBack,
        CharacterPartType.AttachmentHipsFront, CharacterPartType.AttachmentHipsLeft, CharacterPartType.AttachmentHipsRight,
        CharacterPartType.AttachmentKneeLeft, CharacterPartType.AttachmentKneeRight, CharacterPartType.AttachmentHead,
        CharacterPartType.Hair, CharacterPartType.EyebrowLeft, CharacterPartType.EyebrowRight, CharacterPartType.AttachmentFace,
        CharacterPartType.FacialHair
    };

    void Start()
    {
        _dbManager = new DatabaseManager();

        GameObject model = Resources.Load<GameObject>("Meshes/SK_BaseModel");
        Material material = Resources.Load<Material>("Materials/M_BaseMaterial");

        _sidekickRuntime = new SidekickRuntime(model, material, null, _dbManager);
        _partLibrary = _sidekickRuntime.PartLibrary;

        InitializeParts(upperBodyParts);
        InitializeParts(lowerBodyParts);
        InitializeParts(headParts);

        UpdateModel();
    }

    private void InitializeParts(List<CharacterPartType> partList)
    {
        foreach (CharacterPartType type in partList)
        {
            if (ExcludedParts.Contains(type) || !_partLibrary.ContainsKey(type))
                continue;

            _availablePartDictionary[type] = _partLibrary[type];
            _partIndexDictionary[type] = _availablePartDictionary[type].Count - 1;
        }
    }

    private void UpdateModel()
    {
        // Create and populate the list of parts to use from the parts list.
        List<SkinnedMeshRenderer> partsToUse = new List<SkinnedMeshRenderer>();

        foreach (KeyValuePair<CharacterPartType, Dictionary<string, string>> entry in _availablePartDictionary)
        {
            int index = _partIndexDictionary[entry.Key];
            string path = entry.Value.Values.ToArray()[index];
            GameObject partContainer = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            partsToUse.Add(partContainer.GetComponentInChildren<SkinnedMeshRenderer>());
        }

        // Check for an existing copy of the model, if it exists, delete it so that we don't end up with duplicates.
        GameObject character = GameObject.Find(_OUTPUT_MODEL_NAME);

        if (character != null)
        {
            Destroy(character);
        }

        // Create a new character using the selected parts using the Sidekicks API.
        character = _sidekickRuntime.CreateCharacter(_OUTPUT_MODEL_NAME, partsToUse, false, true);

        if (character != null)
        {
            AddScriptAndAnimator(character);
        }
        else
        {
            Debug.LogError("Character creation failed.");
        }
    }
    private void AddScriptAndAnimator(GameObject character)
    {
        if (character == null) return;

        Animator animator = character.GetComponent<Animator>() ?? character.AddComponent<Animator>();
        RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("IdleState");

        CharacterRotation rotationScript = character.GetComponent<CharacterRotation>() ?? character.AddComponent<CharacterRotation>();
        rotationScript.character = character;

        if (controller != null)
        {
            animator.runtimeAnimatorController = controller;
        }
        else
        {
            Debug.LogError("Animator Controller not found at path: Assets/Noak/Animations/IdleState");
        }
    }

    public void Gender()
    {
        if (_sidekickRuntime != null)
        {
            _sidekickRuntime.BodyTypeBlendValue = _sidekickRuntime.BodyTypeBlendValue == 100 ? 0 : 100;

            UpdateModel();
        }
    }
    public void UpdateBodyComposition()
    {
        if (_sidekickRuntime != null)
        {
            _sidekickRuntime.BodySizeSkinnyBlendValue = BodySizeSkinnyBlendValue;
            _sidekickRuntime.BodySizeHeavyBlendValue = BodySizeHeavyBlendValue;
            _sidekickRuntime.MusclesBlendValue = MusclesBlendValue;

            UpdateModel();
        }
    }

    private void ChangePart(CharacterPartType partType, bool forward)
    {
        if (!_availablePartDictionary.ContainsKey(partType))
        {
            _availablePartDictionary[partType] = _partLibrary[partType];
            _partIndexDictionary[partType] = 0;
        }

        int index = _partIndexDictionary[partType];
        index = forward ? (index + 1) % _availablePartDictionary[partType].Count
                        : (index - 1 + _availablePartDictionary[partType].Count) % _availablePartDictionary[partType].Count;

        _partIndexDictionary[partType] = index;
        UpdateModel();
    }
    private void ChangePairedParts(CharacterPartType leftPart, CharacterPartType rightPart, bool forward)
    {
        if (!_availablePartDictionary.ContainsKey(leftPart))
        {
            _availablePartDictionary[leftPart] = _partLibrary[leftPart];
            _partIndexDictionary[leftPart] = 0;
        }
        if (!_availablePartDictionary.ContainsKey(rightPart))
        {
            _availablePartDictionary[rightPart] = _partLibrary[rightPart];
            _partIndexDictionary[rightPart] = 0;
        }

        int index = _partIndexDictionary[leftPart];
        int index2 = _partIndexDictionary[rightPart];

        index = forward ? (index + 1) % _availablePartDictionary[leftPart].Count
                        : (index - 1 + _availablePartDictionary[leftPart].Count) % _availablePartDictionary[leftPart].Count;

        index2 = forward ? (index2 + 1) % _availablePartDictionary[rightPart].Count
                         : (index2 - 1 + _availablePartDictionary[rightPart].Count) % _availablePartDictionary[rightPart].Count;

        _partIndexDictionary[leftPart] = index;
        _partIndexDictionary[rightPart] = index2;
        UpdateModel();
    }

    //Head Parts
    public void ForwardHair() => ChangePart(CharacterPartType.Hair, true);
    public void BackwardHair() => ChangePart(CharacterPartType.Hair, false);
    public void ForwardEyebrows() => ChangePairedParts(CharacterPartType.EyebrowLeft, CharacterPartType.EyebrowRight, true);
    public void BackwardEyebrows() => ChangePairedParts(CharacterPartType.EyebrowLeft, CharacterPartType.EyebrowRight, false);
    public void ForwardEars() => ChangePairedParts(CharacterPartType.EarLeft, CharacterPartType.EarRight, true);
    public void BackwardEars() => ChangePairedParts(CharacterPartType.EarLeft, CharacterPartType.EarRight, false);
    public void ForwardTeeth() => ChangePart(CharacterPartType.Teeth, true);
    public void BackwardTeeth() => ChangePart(CharacterPartType.Teeth, false);
    public void ForwardFacialHair() => ChangePart(CharacterPartType.FacialHair, true);
    public void BackwardFacialHair() => ChangePart(CharacterPartType.FacialHair, false);

    //Upper Body parts
    public void ForwardTorso() => ChangePart(CharacterPartType.Torso, true);
    public void BackwardTorso() => ChangePart(CharacterPartType.Torso, false);
    public void ForwardUpperArm() => ChangePairedParts(CharacterPartType.ArmUpperLeft, CharacterPartType.ArmUpperRight, true);
    public void BackwardUpperArm() => ChangePairedParts(CharacterPartType.ArmUpperLeft, CharacterPartType.ArmUpperRight, false);
    public void ForwardLowerArm() => ChangePairedParts(CharacterPartType.ArmLowerLeft, CharacterPartType.ArmLowerRight, true);
    public void BackwardLowerArm() => ChangePairedParts(CharacterPartType.ArmLowerLeft, CharacterPartType.ArmLowerRight, false);
    public void ForwardHand() => ChangePairedParts(CharacterPartType.HandLeft, CharacterPartType.HandRight, true);
    public void BackwardHand() => ChangePairedParts(CharacterPartType.HandLeft, CharacterPartType.HandRight, false);

    //Lower Body parts
    public void ForwardHips() => ChangePart(CharacterPartType.Hips, true);
    public void BackwardHips() => ChangePart(CharacterPartType.Hips, false);
    public void ForwardLeg() => ChangePairedParts(CharacterPartType.LegLeft, CharacterPartType.LegRight, true);
    public void BackwardLeg() => ChangePairedParts(CharacterPartType.LegLeft, CharacterPartType.LegRight, false);
    public void ForwardFoot() => ChangePairedParts(CharacterPartType.FootLeft, CharacterPartType.FootRight, true);
    public void BackwardFoot() => ChangePairedParts(CharacterPartType.FootLeft, CharacterPartType.FootRight, false);
}
