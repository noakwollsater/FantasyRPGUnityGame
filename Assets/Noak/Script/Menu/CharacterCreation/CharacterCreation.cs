using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Enums;
using Synty.SidekickCharacters.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CharacterCreation : MonoBehaviour
{
    private readonly string _OUTPUT_MODEL_NAME = "Sidekick Character";

    public DatabaseManager _dbManager;
    public SidekickRuntime _sidekickRuntime;
    public DictionaryLibrary _dictionaryLibrary;

    // For this example we are only interested in Upper Body parts, so we filter the list of all parts to only get the ones we want.
    public List<CharacterPartType> upperBodyParts = PartGroup.UpperBody.GetPartTypes();
    public List<CharacterPartType> lowerBodyParts = PartGroup.LowerBody.GetPartTypes();
    public List<CharacterPartType> headParts = PartGroup.Head.GetPartTypes();

    public static readonly HashSet<CharacterPartType> ExcludedParts = new()
    {
        CharacterPartType.AttachmentBack, CharacterPartType.AttachmentElbowLeft, CharacterPartType.AttachmentElbowRight,
        CharacterPartType.AttachmentShoulderLeft, CharacterPartType.AttachmentShoulderRight, CharacterPartType.AttachmentHipsBack,
        CharacterPartType.AttachmentHipsFront, CharacterPartType.AttachmentHipsLeft, CharacterPartType.AttachmentHipsRight,
        CharacterPartType.AttachmentKneeLeft, CharacterPartType.AttachmentKneeRight, CharacterPartType.AttachmentHead,
        CharacterPartType.Hair, CharacterPartType.EyebrowLeft, CharacterPartType.EyebrowRight, CharacterPartType.AttachmentFace,
        CharacterPartType.FacialHair
    };

    //Ear and Nose stuff
    public bool isNose;

    protected virtual void Start()
    {
        _dbManager = new DatabaseManager();

        GameObject model = Resources.Load<GameObject>("Meshes/SK_BaseModel");
        Material material = Resources.Load<Material>("Materials/M_BaseMaterial");

        _sidekickRuntime = new SidekickRuntime(model, material, null, _dbManager);
        _dictionaryLibrary._partLibrary = _sidekickRuntime.PartLibrary;

        // Defer to a default species selection (if derived class doesn't override)
        if (this is SpeciesChooser chooser)
        {
            chooser.SelectSpecies("Human");
        }
    }


    public void InitializeParts(List<CharacterPartType> partList)
    {
        foreach (CharacterPartType type in partList)
        {
            if (ExcludedParts.Contains(type) || !_dictionaryLibrary._partLibrary.ContainsKey(type))
                continue;

            // Check if the body part has a restricted list
            if (_dictionaryLibrary.AllowedParts.ContainsKey(type))
            {
                var filteredParts = _dictionaryLibrary._partLibrary[type]
                    .Where(kvp => _dictionaryLibrary.AllowedParts[type].Contains(kvp.Key)) // Filter based on allowed parts
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                _dictionaryLibrary._availablePartDictionary[type] = filteredParts;
            }
            else
            {
                _dictionaryLibrary._availablePartDictionary[type] = _dictionaryLibrary._partLibrary[type];
            }

            _dictionaryLibrary._partIndexDictionary[type] = _dictionaryLibrary._availablePartDictionary[type].Count - 1;
        }
    }

    public void UpdateModel()
    {
        // Create and populate the list of parts to use
        List<SkinnedMeshRenderer> partsToUse = new List<SkinnedMeshRenderer>();

        foreach (KeyValuePair<CharacterPartType, Dictionary<string, string>> entry in _dictionaryLibrary._availablePartDictionary)
        {
            int index = _dictionaryLibrary._partIndexDictionary[entry.Key];
            string resourcePath = entry.Value.Values.ToArray()[index];
            GameObject partContainer = Resources.Load<GameObject>(resourcePath);

            partsToUse.Add(partContainer.GetComponentInChildren<SkinnedMeshRenderer>());
        }

        // Check for an existing copy of the model, delete it to avoid duplicates
        GameObject character = GameObject.Find(_OUTPUT_MODEL_NAME);

        if (character != null)
        {
            Destroy(character);
        }

        SetSize();
        // Create a new character using the selected parts
        character = _sidekickRuntime.CreateCharacter(_OUTPUT_MODEL_NAME, partsToUse, false, true);

        if (character != null)
        {
            AddScriptAndAnimator(character);
            SetSize();

            Debug.Log($"Applying BodyTypeBlendValue: {_sidekickRuntime.BodyTypeBlendValue}");
        }
        else
        {
            Debug.LogError("Character creation failed.");
        }
    }

    private void SetSize()
    {
        _sidekickRuntime.BodyTypeBlendValue = _dictionaryLibrary.BodyTypeBlendValue;
        _sidekickRuntime.BodySizeHeavyBlendValue = _dictionaryLibrary.BodySizeHeavyBlendValue;
        _sidekickRuntime.BodySizeSkinnyBlendValue = _dictionaryLibrary.BodySizeSkinnyBlendValue;
        _sidekickRuntime.MusclesBlendValue = _dictionaryLibrary.MusclesBlendValue;
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

    private void ChangePart(CharacterPartType partType, bool forward)
    {
        if (!_dictionaryLibrary._availablePartDictionary.ContainsKey(partType))
        {
            _dictionaryLibrary._availablePartDictionary[partType] = _dictionaryLibrary._partLibrary[partType];
            _dictionaryLibrary._partIndexDictionary[partType] = 0;
        }

        var availableParts = _dictionaryLibrary._availablePartDictionary[partType];

        if (availableParts.Count == 0)
        {
            Debug.LogWarning($"No available parts for {partType}");
            return;
        }

        int index = _dictionaryLibrary._partIndexDictionary[partType];

        do
        {
            index = forward
                ? (index + 1) % availableParts.Count
                : (index - 1 + availableParts.Count) % availableParts.Count;

            string selectedPartName = availableParts.Keys.ElementAt(index);

            // Ensure the part follows the allowed list if applicable
            if (!_dictionaryLibrary.AllowedParts.ContainsKey(partType) || _dictionaryLibrary.AllowedParts[partType].Contains(selectedPartName))
            {
                break;
            }

        } while (true);

        _dictionaryLibrary._partIndexDictionary[partType] = index;
        UpdateModel();
    }


    private void ChangePairedParts(CharacterPartType leftPart, CharacterPartType rightPart, bool forward)
    {
        if (!_dictionaryLibrary._availablePartDictionary.ContainsKey(leftPart) ||
            !_dictionaryLibrary._availablePartDictionary.ContainsKey(rightPart))
        {
            _dictionaryLibrary._availablePartDictionary[leftPart] = _dictionaryLibrary._partLibrary[leftPart];
            _dictionaryLibrary._availablePartDictionary[rightPart] = _dictionaryLibrary._partLibrary[rightPart];

            _dictionaryLibrary._partIndexDictionary[leftPart] = 0;
            _dictionaryLibrary._partIndexDictionary[rightPart] = 0;
        }

        var availableLeftParts = _dictionaryLibrary._availablePartDictionary[leftPart];
        var availableRightParts = _dictionaryLibrary._availablePartDictionary[rightPart];

        if (availableLeftParts.Count == 0 || availableRightParts.Count == 0)
        {
            Debug.LogWarning($"No available parts for {leftPart} or {rightPart}");
            return;
        }

        int leftIndex = _dictionaryLibrary._partIndexDictionary[leftPart];
        int rightIndex = _dictionaryLibrary._partIndexDictionary[rightPart];

        do
        {
            leftIndex = forward
                ? (leftIndex + 1) % availableLeftParts.Count
                : (leftIndex - 1 + availableLeftParts.Count) % availableLeftParts.Count;

            rightIndex = forward
                ? (rightIndex + 1) % availableRightParts.Count
                : (rightIndex - 1 + availableRightParts.Count) % availableRightParts.Count;

            string leftSelectedPartName = availableLeftParts.Keys.ElementAt(leftIndex);
            string rightSelectedPartName = availableRightParts.Keys.ElementAt(rightIndex);

            // Ensure both left and right parts are allowed if there are restrictions
            bool isLeftAllowed = !_dictionaryLibrary.AllowedParts.ContainsKey(leftPart) || _dictionaryLibrary.AllowedParts[leftPart].Contains(leftSelectedPartName);
            bool isRightAllowed = !_dictionaryLibrary.AllowedParts.ContainsKey(rightPart) || _dictionaryLibrary.AllowedParts[rightPart].Contains(rightSelectedPartName);

            if (isLeftAllowed && isRightAllowed)
            {
                break;
            }

        } while (true);

        _dictionaryLibrary._partIndexDictionary[leftPart] = leftIndex;
        _dictionaryLibrary._partIndexDictionary[rightPart] = rightIndex;
        UpdateModel();
    }

    public void ForwardEarOrNose()
    {
        Debug.Log($"ForwardEarOrNose: isNose={isNose}");

        if (isNose)
        {
            Debug.Log("Calling ForwardNose()");
            ForwardNose();
        }
        else
        {
            Debug.Log("Calling ForwardEars()");
            ForwardEars();
        }
    }

    public void BackwardEarOrNose()
    {
        Debug.Log($"BackwardEarOrNose: isNose={isNose}");

        if (isNose)
        {
            Debug.Log("Calling BackwardNose()");
            BackwardNose();
        }
        else
        {
            Debug.Log("Calling BackwardEars()");
            BackwardEars();
        }
    }


    //Head Parts
    public void ForwardHair() => ChangePart(CharacterPartType.Hair, true);
    public void BackwardHair() => ChangePart(CharacterPartType.Hair, false);
    public void ForwardEyebrows() => ChangePairedParts(CharacterPartType.EyebrowLeft, CharacterPartType.EyebrowRight, true);
    public void BackwardEyebrows() => ChangePairedParts(CharacterPartType.EyebrowLeft, CharacterPartType.EyebrowRight, false);
    private void ForwardEars() => ChangePairedParts(CharacterPartType.EarLeft, CharacterPartType.EarRight, true);
    private void BackwardEars() => ChangePairedParts(CharacterPartType.EarLeft, CharacterPartType.EarRight, false);
    private void ForwardNose() => ChangePart(CharacterPartType.Nose, true);
    private void BackwardNose() => ChangePart(CharacterPartType.Nose, false);
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
