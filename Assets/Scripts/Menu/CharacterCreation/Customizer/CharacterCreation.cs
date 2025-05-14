using FIMSpace.FTail;
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Enums;
using Synty.SidekickCharacters.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterCreation : MonoBehaviour
{
    private readonly string _OUTPUT_MODEL_NAME = "Sidekick Character";

    public DatabaseManager _dbManager;
    public SidekickRuntime _sidekickRuntime;
    public DictionaryLibrary _dictionaryLibrary;

    private Material _addressableMaterial;

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
        CharacterPartType.FacialHair, CharacterPartType.Wrap
    };

    //Ear and Nose stuff
    public bool isNose;
    public Transform root;

    protected virtual void Start()
    {
        _dbManager = new DatabaseManager();

        CharacterRuntimeManager.InitIfNeeded();
        _sidekickRuntime = CharacterRuntimeManager.RuntimeInstance;
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

            root = character.transform.Find("root");
            if (root != null)
            {
                AttachTailAnimators(root);
            }
            else
            {
                Debug.LogError("Root transform not found in character model.");
            }

            Debug.Log($"Applying BodyTypeBlendValue: {_sidekickRuntime.BodyTypeBlendValue}");
        }
        else
        {
            Debug.LogError("Character creation failed.");
        }
    }

    public void AttachTailAnimators(Transform root)
    {
        if (root == null)
        {
            Debug.LogWarning("AttachTailAnimators: root is null");
            return;
        }

        // Traverse all children in the hierarchy
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child == null) continue;

            string name = child.name?.ToLower();
            if (!string.IsNullOrEmpty(name) && name.Contains("dyn"))
            {
                if (!child.TryGetComponent<TailAnimator2>(out _))
                {
                    child.gameObject.AddComponent<TailAnimator2>();
                    TailAnimator2 tailAnimator = child.GetComponent<TailAnimator2>();
                    ConfigureTailAnimator(tailAnimator);
                    Debug.Log($"TailAnimator2 added to {child.name}");
                }
            }
        }
    }

    private void ConfigureTailAnimator(TailAnimator2 tail)
    {
        if (tail == null) return;

        tail.UseWaving = false;
        tail.MotionInfluence = 1.5f;
        tail.ReactionSpeed = 1f;
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

    public void RemoveHair()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.Hair))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.Hair);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.Hair);
        }
        UpdateModel();
    }
    public void RemoveEyebrows()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.EyebrowLeft))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.EyebrowLeft);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.EyebrowLeft);
        }
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.EyebrowRight))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.EyebrowRight);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.EyebrowRight);
        }
        UpdateModel();
    }
    public void RemoveFacialHair()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.FacialHair))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.FacialHair);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.FacialHair);
        }
        UpdateModel();
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

    // Attachments
    public void ForwardAttachmentBack() => ChangePart(CharacterPartType.AttachmentBack, true);
    public void BackwardAttachmentBack() => ChangePart(CharacterPartType.AttachmentBack, false);
    public void RemoveBackAttachment()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentBack))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentBack);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentBack);
        }
        UpdateModel();
    }

    public void ForwardAttachmentHelmet() => ChangePart(CharacterPartType.AttachmentHead, true);
    public void BackwardAttachmentHelmet() => ChangePart(CharacterPartType.AttachmentHead, false);
    public void RemoveHeadAttachment()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentHead))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentHead);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentHead);
        }
        UpdateModel();
    }

    public void ForwardAttachmentFace() => ChangePart(CharacterPartType.AttachmentFace, true);
    public void BackwardAttachmentFace() => ChangePart(CharacterPartType.AttachmentFace, false);
    public void RemoveFaceAttachment()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentFace))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentFace);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentFace);
        }
        UpdateModel();
    }

    public void ForwardAttachmentShoulder() => ChangePairedParts(CharacterPartType.AttachmentShoulderLeft, CharacterPartType.AttachmentShoulderRight, true);
    public void BackwardAttachmentShoulder() => ChangePairedParts(CharacterPartType.AttachmentShoulderLeft, CharacterPartType.AttachmentShoulderRight, false);
    public void RemoveShoulderAttachment()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentShoulderLeft))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentShoulderLeft);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentShoulderLeft);
        }
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentShoulderRight))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentShoulderRight);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentShoulderRight);
        }
        UpdateModel();
    }

    public void ForwardAttachmentElbow() => ChangePairedParts(CharacterPartType.AttachmentElbowLeft, CharacterPartType.AttachmentElbowRight, true);
    public void BackwardAttachmentElbow() => ChangePairedParts(CharacterPartType.AttachmentElbowLeft, CharacterPartType.AttachmentElbowRight, false);
    public void RemoveElbowAttachment()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentElbowLeft))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentElbowLeft);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentElbowLeft);
        }
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentElbowRight))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentElbowRight);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentElbowRight);
        }
        UpdateModel();
    }

    public void ForwardAttachmentHipRight() => ChangePart(CharacterPartType.AttachmentHipsRight, true);
    public void BackwardAttachmentHipRight() => ChangePart(CharacterPartType.AttachmentHipsRight, false);
    public void RemoveHipAttachment()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentHipsRight))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentHipsRight);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentHipsRight);
        }
        UpdateModel();
    }

    public void ForwardAttachmentHipLeft() => ChangePart(CharacterPartType.AttachmentHipsLeft, true);
    public void BackwardAttachmentHipLeft() => ChangePart(CharacterPartType.AttachmentHipsLeft, false);
    public void RemoveHipAttachmentLeft()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentHipsLeft))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentHipsLeft);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentHipsLeft);
        }
        UpdateModel();
    }

    public void ForwardAttachmentHipFront() => ChangePart(CharacterPartType.AttachmentHipsFront, true);
    public void BackwardAttachmentHipFront() => ChangePart(CharacterPartType.AttachmentHipsFront, false);
    public void RemoveHipAttachmentFront()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentHipsFront))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentHipsFront);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentHipsFront);
        }
        UpdateModel();
    }

    public void ForwardAttachmentHipBack() => ChangePart(CharacterPartType.AttachmentHipsBack, true);
    public void BackwardAttachmentHipBack() => ChangePart(CharacterPartType.AttachmentHipsBack, false);
    public void RemoveHipAttachmentBack()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentHipsBack))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentHipsBack);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentHipsBack);
        }
        UpdateModel();
    }

    public void ForwardAttachmentKnee() => ChangePairedParts(CharacterPartType.AttachmentKneeLeft, CharacterPartType.AttachmentKneeRight, true);
    public void BackwardAttachmentKnee() => ChangePairedParts(CharacterPartType.AttachmentKneeLeft, CharacterPartType.AttachmentKneeRight, false);
    public void RemoveKneeAttachment()
    {
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentKneeLeft))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentKneeLeft);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentKneeLeft);
        }
        if (_dictionaryLibrary._availablePartDictionary.ContainsKey(CharacterPartType.AttachmentKneeRight))
        {
            _dictionaryLibrary._availablePartDictionary.Remove(CharacterPartType.AttachmentKneeRight);
            _dictionaryLibrary._partIndexDictionary.Remove(CharacterPartType.AttachmentKneeRight);
        }
        UpdateModel();
    }
}
