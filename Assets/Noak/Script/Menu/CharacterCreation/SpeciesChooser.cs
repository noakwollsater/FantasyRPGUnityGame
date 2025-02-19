using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpeciesChooser : CharacterCreation
{
    private List<SidekickSpecies> _availableSpecies;
    private int _currentSpeciesIndex = 0;
    private SidekickSpecies _selectedSpecies;

    [SerializeField] private Sprite[] RaceHeadIcons;
    [SerializeField] private Image headIcon;

    void Start()
    {
        _dbManager = new DatabaseManager();

        if (_dbManager.GetCurrentDbConnection() == null)
        {
            Debug.LogError("Database connection failed to initialize.");
            return;
        }

        _availableSpecies = SidekickSpecies.GetAll(_dbManager);
        if (_availableSpecies == null || _availableSpecies.Count == 0)
        {
            Debug.LogError("No species found in the database.");
            return;
        }

        // ✅ Ensure _selectedSpecies is set
        _selectedSpecies = _availableSpecies[_currentSpeciesIndex];
        Debug.Log($"Initialized species: {_selectedSpecies.Name}");

        UpdateImage();

        // Ensure SidekickRuntime is initialized
        GameObject model = Resources.Load<GameObject>("Meshes/SK_BaseModel");
        Material material = Resources.Load<Material>("Materials/M_BaseMaterial");

        _sidekickRuntime = new SidekickRuntime(model, material, null, _dbManager);
        _dictionaryLibrary._partLibrary = _sidekickRuntime?.PartLibrary;

        if (_dictionaryLibrary._partLibrary == null)
        {
            Debug.LogError("SidekickRuntime failed to initialize part library.");
            return;
        }

        UpdateSpecies();
    }

    private void UpdateImage()
    {
        int index = _availableSpecies.IndexOf(_selectedSpecies);

        if (index == -1)
        {
            Debug.LogError($"UpdateImage: Selected species '{_selectedSpecies.Name}' not found in available species list.");
            return;
        }

        _currentSpeciesIndex = index; // ✅ Ensure the index is updated

        if (RaceHeadIcons.Length > _currentSpeciesIndex)
        {
            headIcon.sprite = RaceHeadIcons[_currentSpeciesIndex];
        }
        else
        {
            Debug.LogError("UpdateImage: No matching head icon found for selected species.");
        }
    }


    public void SelectSpecies(string speciesName)
    {
        if (_availableSpecies == null || _availableSpecies.Count == 0)
        {
            Debug.LogError("No species available.");
            return;
        }

        // Find the species by name
        _selectedSpecies = _availableSpecies.FirstOrDefault(s => s.Name == speciesName);

        if (_selectedSpecies == null)
        {
            Debug.LogError($"Species '{speciesName}' not found.");
            return;
        }

        Debug.Log($"Selected species: {_selectedSpecies.Name}");
        UpdateImage();
        UpdateSpecies();
    }
    private void UpdateSpecies()
    {
        if (_selectedSpecies == null)
        {
            Debug.LogError("UpdateSpecies() failed: _selectedSpecies is null.");
            return;
        }

        if (_dictionaryLibrary._partLibrary == null)
        {
            Debug.LogError("UpdateSpecies() failed: _partLibrary is null. Ensure SidekickRuntime is initialized.");
            return;
        }

        Debug.Log($"Updating model for species: {_selectedSpecies.Name}");

        // **Step 1: Reset Model to Base**
        GameObject baseModel = Resources.Load<GameObject>("Meshes/SK_BaseModel");
        Material baseMaterial = Resources.Load<Material>("Materials/M_BaseMaterial");

        _sidekickRuntime = new SidekickRuntime(baseModel, baseMaterial, null, _dbManager);

        if (_sidekickRuntime == null)
        {
            Debug.LogError("Failed to reset SidekickRuntime.");
            return;
        }

        _dictionaryLibrary._partLibrary = _sidekickRuntime.PartLibrary;

        // **Step 2: Ensure dictionaries are reset**
        _dictionaryLibrary._availablePartDictionary.Clear();
        _dictionaryLibrary._partIndexDictionary.Clear();

        // **Step 3: Explicitly Add `_BASE_` First**
        bool hasBasePart = false;

        foreach (var partType in _dictionaryLibrary._partLibrary.Keys)
        {
            if (ExcludedParts.Contains(partType)) continue;

            var partsForSpecies = _dictionaryLibrary._partLibrary[partType]
                .Where(part => SidekickPart.GetSpeciesForPart(_availableSpecies, part.Key) == _selectedSpecies)
                .ToDictionary(p => p.Key, p => p.Value);

            if (partsForSpecies.Count > 0)
            {
                _dictionaryLibrary._availablePartDictionary[partType] = partsForSpecies;
                _dictionaryLibrary._partIndexDictionary[partType] = 0;

                // **Explicitly Check & Apply `_BASE_`**
                string basePartKey = partsForSpecies.Keys.FirstOrDefault(k => k.Contains("_BASE_"));
                if (basePartKey != null)
                {
                    _dictionaryLibrary._partIndexDictionary[partType] = partsForSpecies.Keys.ToList().IndexOf(basePartKey);
                    hasBasePart = true;
                    Debug.Log($"_BASE_ found and set for {partType} in species {_selectedSpecies.Name}");
                }
            }
        }

        // **Step 4: Error Handling if `_BASE_` Missing**
        if (!hasBasePart)
        {
            Debug.LogError($"Species {_selectedSpecies.Name} is missing a '_BASE_' part! Defaulting to first available part.");
        }

        // **Step 5: Apply Species Colors**
        ApplySpeciesColors(_selectedSpecies.Name);

        // **Step 6: Apply Changes**
        UpdateModel();
    }

    private void ApplySpeciesColors(string species)
    {
        if (_sidekickRuntime == null)
        {
            Debug.LogError("ApplySpeciesColors: SidekickRuntime is not initialized.");
            return;
        }

        if (!_dictionaryLibrary.speciesColors.TryGetValue(species, out var speciesData))
        {
            Debug.LogWarning($"No color data found for species: {species}");
            return;
        }

        foreach (var entry in speciesData)
        {
            if (_dictionaryLibrary.bodyPartMappings.TryGetValue(entry.Key, out var targetParts))
            {
                ApplyColorToParts(entry.Value, targetParts);
            }
        }
    }

    private void ApplyColorToParts(Color color, List<string> targetParts)
    {
        if (_sidekickRuntime == null)
        {
            Debug.LogError("ApplyColorToParts: SidekickRuntime is not initialized.");
            return;
        }

        List<SidekickColorProperty> allProperties = SidekickColorProperty.GetAll(_dbManager);
        List<SidekickColorProperty> selectedProperties = allProperties
            .Where(scp => targetParts.Any(part => scp.Name.ToLower().Contains(part.ToLower())))
            .ToList();

        if (selectedProperties.Count == 0)
        {
            Debug.LogWarning($"No matching properties found for {string.Join(", ", targetParts)}.");
            return;
        }

        foreach (SidekickColorProperty property in selectedProperties)
        {
            SidekickColorRow row = new SidekickColorRow
            {
                ColorProperty = property,
                MainColor = ColorUtility.ToHtmlStringRGB(color),
            };

            _sidekickRuntime.UpdateColor(ColorType.MainColor, row);
            Debug.Log($"Applied {color} to {property.Name}");
        }
    }

    public void ChooseHuman() => SelectSpecies("Human");
    public void ChooseGoblin() => SelectSpecies("Goblin");
    public void ChooseOrc() => SelectSpecies("Orc");
    public void ChooseElf() => SelectSpecies("Elf");
    public void ChooseDwarf() => SelectSpecies("Dwarf");
    public void ChooseDarkelf() => SelectSpecies("Darkelf");
    public void ChooseDrakonit() => SelectSpecies("Draknoit");
    public void ChooseLynx() => SelectSpecies("Lynx");
    public void ChooseHobbit() => SelectSpecies("Hobbit");
    public void ChooseApe() => SelectSpecies("Ape");

}
