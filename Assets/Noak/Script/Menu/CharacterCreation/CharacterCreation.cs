using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using Synty.SidekickCharacters.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Synty.SidekickCharacters.Demo
{
    /// <summary>
    ///     An example script to show how to interact with the Sidekick API in regards to parts at runtime.
    /// </summary>
    public class CharacterCreation : MonoBehaviour
    {
        private readonly string _OUTPUT_MODEL_NAME = "Sidekick Character";

        Dictionary<CharacterPartType, int> _partIndexDictionary = new Dictionary<CharacterPartType, int>();
        Dictionary<CharacterPartType, Dictionary<string, string>> _availablePartDictionary = new Dictionary<CharacterPartType, Dictionary<string, string>>();

        private DatabaseManager _dbManager;
        private SidekickRuntime _sidekickRuntime;

        private Dictionary<CharacterPartType, Dictionary<string, string>> _partLibrary;

        /// <inheritdoc cref="Start"/>
        void Start()
        {
            // Create a new instance of the database manager to access database content.
            _dbManager = new DatabaseManager();

            // Load the base model and material required to create an instance of the Sidekick Runtime API.
            GameObject model = Resources.Load<GameObject>("Meshes/SK_BaseModel");
            Material material = Resources.Load<Material>("Materials/M_BaseMaterial");

            _sidekickRuntime = new SidekickRuntime(model, material, null, _dbManager);

            // Populate the parts list for easy access.
            _partLibrary = _sidekickRuntime.PartLibrary;

            // For this example we are only interested in Upper Body parts, so we filter the list of all parts to only get the ones we want.
            List<CharacterPartType> upperBodyParts = PartGroup.UpperBody.GetPartTypes();
            List<CharacterPartType> lowerBodyParts = PartGroup.LowerBody.GetPartTypes();
            List<CharacterPartType> headParts = PartGroup.Head.GetPartTypes();

            foreach (CharacterPartType type in upperBodyParts)
            {
                _availablePartDictionary.Add(type, _partLibrary[type]);
                _partIndexDictionary.Add(type, Random.Range(0, _availablePartDictionary[type].Count - 1));
            }
            foreach (CharacterPartType type in lowerBodyParts)
            {
                _availablePartDictionary.Add(type, _partLibrary[type]);
                _partIndexDictionary.Add(type, Random.Range(0, _availablePartDictionary[type].Count - 1));
            }
            foreach (CharacterPartType type in headParts)
            {
                _availablePartDictionary.Add(type, _partLibrary[type]);
                _partIndexDictionary.Add(type, Random.Range(0, _availablePartDictionary[type].Count - 1));
            }


            UpdateModel();
        }
        
        //Head Parts
        public void ForwardHair()
        {
            int index = _partIndexDictionary[CharacterPartType.Hair];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.Hair].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.Hair] = index;
            UpdateModel();
        }
        public void BackHair()
        {
            int index = _partIndexDictionary[CharacterPartType.Hair];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.Hair].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.Hair] = index;
            UpdateModel();
        }

        public void ForwardEyebrows()
        {
            int index = _partIndexDictionary[CharacterPartType.EyebrowLeft];
            int index2 = _partIndexDictionary[CharacterPartType.EyebrowRight];
            index++;
            index2++;

            if (index >= _availablePartDictionary[CharacterPartType.EyebrowLeft].Count)
            {
                index = 0;
            }
            if (index2 >= _availablePartDictionary[CharacterPartType.EyebrowRight].Count)
            {
                index2 = 0;
            }

            _partIndexDictionary[CharacterPartType.EyebrowLeft] = index;
            _partIndexDictionary[CharacterPartType.EyebrowRight] = index2;
            UpdateModel();
        }
        public void BackwardEyebrows()
        {
            int index = _partIndexDictionary[CharacterPartType.EyebrowLeft];
            int index2 = _partIndexDictionary[CharacterPartType.EyebrowRight];
            index--;
            index2--;

            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.EyebrowLeft].Count - 1;
            }
            if (index2 < 0)
            {
                index2 = _availablePartDictionary[CharacterPartType.EyebrowRight].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.EyebrowLeft] = index;
            _partIndexDictionary[CharacterPartType.EyebrowRight] = index2;
            UpdateModel();
        }

        public void ForwardEars()
        {
            int index = _partIndexDictionary[CharacterPartType.EarLeft];
            int index2 = _partIndexDictionary[CharacterPartType.EarRight];
            index++;
            index2++;

            if (index >= _availablePartDictionary[CharacterPartType.EarLeft].Count)
            {
                index = 0;
            }
            if (index2 >= _availablePartDictionary[CharacterPartType.EarRight].Count)
            {
                index2 = 0;
            }

            _partIndexDictionary[CharacterPartType.EarLeft] = index;
            _partIndexDictionary[CharacterPartType.EarRight] = index2;
            UpdateModel();
        }
        public void BackwardEars()
        {
            int index = _partIndexDictionary[CharacterPartType.EarLeft];
            int index2 = _partIndexDictionary[CharacterPartType.EarRight];
            index--;
            index2--;

            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.EarLeft].Count - 1;
            }
            if (index2 < 0)
            {
                index2 = _availablePartDictionary[CharacterPartType.EarRight].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.EarLeft] = index;
            _partIndexDictionary[CharacterPartType.EarRight] = index2;
            UpdateModel();
        }

        public void ForwardTeeth()
        {
            int index = _partIndexDictionary[CharacterPartType.Teeth];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.Teeth].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.Teeth] = index;
            UpdateModel();
        }
        public void BackwardTeeth()
        {
            int index = _partIndexDictionary[CharacterPartType.Teeth];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.Teeth].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.Teeth] = index;
            UpdateModel();
        }



        public void ForwardFacialHair()
        {
            int index = _partIndexDictionary[CharacterPartType.FacialHair];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.FacialHair].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.FacialHair] = index;
            UpdateModel();
        }
        public void BackwardFacialHair()
        {
            int index = _partIndexDictionary[CharacterPartType.FacialHair];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.FacialHair].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.FacialHair] = index;
            UpdateModel();
        }



        //Lower Body parts
        public void ForwardTorso()
        {
            int index = _partIndexDictionary[CharacterPartType.Torso];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.Torso].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.Torso] = index;
            UpdateModel();
        }

        public void BackwardTorso()
        {
            int index = _partIndexDictionary[CharacterPartType.Torso];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.Torso].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.Torso] = index;
            UpdateModel();
        }

        public void ForwardUpperArmLeft()
        {
            int index = _partIndexDictionary[CharacterPartType.ArmUpperLeft];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.ArmUpperLeft].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.ArmUpperLeft] = index;
            UpdateModel();
        }

        public void BackwardUpperArmLeft()
        {
            int index = _partIndexDictionary[CharacterPartType.ArmUpperLeft];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.ArmUpperLeft].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.ArmUpperLeft] = index;
            UpdateModel();
        }

        public void ForwardUpperArmRight()
        {
            int index = _partIndexDictionary[CharacterPartType.ArmUpperRight];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.ArmUpperRight].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.ArmUpperRight] = index;
            UpdateModel();
        }

        public void BackwardUpperArmRight()
        {
            int index = _partIndexDictionary[CharacterPartType.ArmUpperRight];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.ArmUpperRight].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.ArmUpperRight] = index;
            UpdateModel();
        }

        public void ForwardLowerArmLeft()
        {
            int index = _partIndexDictionary[CharacterPartType.ArmLowerLeft];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.ArmLowerLeft].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.ArmLowerLeft] = index;
            UpdateModel();
        }

        public void BackwardLowerArmLeft()
        {
            int index = _partIndexDictionary[CharacterPartType.ArmLowerLeft];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.ArmLowerLeft].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.ArmLowerLeft] = index;
            UpdateModel();
        }

        public void ForwardLowerArmRight()
        {
            int index = _partIndexDictionary[CharacterPartType.ArmLowerRight];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.ArmLowerRight].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.ArmLowerRight] = index;
            UpdateModel();
        }

        public void BackwardLowerArmRight()
        {
            int index = _partIndexDictionary[CharacterPartType.ArmLowerRight];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.ArmLowerRight].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.ArmLowerRight] = index;
            UpdateModel();
        }

        public void ForwardHandLeft()
        {
            int index = _partIndexDictionary[CharacterPartType.HandLeft];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.HandLeft].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.HandLeft] = index;
            UpdateModel();
        }

        public void BackwardHandLeft()
        {
            int index = _partIndexDictionary[CharacterPartType.HandLeft];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.HandLeft].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.HandLeft] = index;
            UpdateModel();
        }

        public void ForwardHandRight()
        {
            int index = _partIndexDictionary[CharacterPartType.HandRight];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.HandRight].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.HandRight] = index;
            UpdateModel();
        }

        public void BackwardHandRight()
        {
            int index = _partIndexDictionary[CharacterPartType.HandRight];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.HandRight].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.HandRight] = index;
            UpdateModel();
        }

        public void ForwardBackAttachment()
        {
            int index = _partIndexDictionary[CharacterPartType.AttachmentBack];
            index++;
            if (index >= _availablePartDictionary[CharacterPartType.AttachmentBack].Count)
            {
                index = 0;
            }

            _partIndexDictionary[CharacterPartType.AttachmentBack] = index;
            UpdateModel();
        }

        public void BackwardBackAttachment()
        {
            int index = _partIndexDictionary[CharacterPartType.AttachmentBack];
            index--;
            if (index < 0)
            {
                index = _availablePartDictionary[CharacterPartType.AttachmentBack].Count - 1;
            }

            _partIndexDictionary[CharacterPartType.AttachmentBack] = index;
            UpdateModel();
        }

        /// <param name="slider">The UI slider to get the values from.</param>
        public void UpdateBodySize(Slider slider)
        {
            // If the slider is greater than 0, then we update the Heavy blend and zero the Skinny.
            if (slider.value > 0)
            {
                _sidekickRuntime.BodySizeHeavyBlendValue = slider.value;
                _sidekickRuntime.BodySizeSkinnyBlendValue = 0;
            }
            // If the slider is 0 or below, we zero the Heavy blend, then we update the Skinny blend.
            else
            {
                _sidekickRuntime.BodySizeHeavyBlendValue = 0;
                _sidekickRuntime.BodySizeSkinnyBlendValue = -slider.value;
            }

            UpdateModel();
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
        }
    }
}
