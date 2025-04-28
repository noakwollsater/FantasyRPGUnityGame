using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SidekickCustomizerUI : MonoBehaviour
{
    [Header("References")]
    public SidekickLoader sidekickLoader;
    public TMP_Dropdown partDropdown;
    public Button leftButton;
    public Button rightButton;
    public TMP_Text variantText;

    private string currentPart;
    private int currentVariantIndex = 0;

    private void Start()
    {
        PopulateDropdown();
        partDropdown.onValueChanged.AddListener(OnPartChanged);
        leftButton.onClick.AddListener(OnLeftClicked);
        rightButton.onClick.AddListener(OnRightClicked);

        if (partDropdown.options.Count > 0)
        {
            currentPart = partDropdown.options[0].text;
            UpdateVariantDisplay();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnLeftClicked();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnRightClicked();
        }
    }

    private void PopulateDropdown()
    {
        partDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (var part in sidekickLoader.allParts)
        {
            options.Add(part.partName);
        }

        partDropdown.AddOptions(options);
    }

    private void OnPartChanged(int index)
    {
        currentPart = partDropdown.options[index].text;
        currentVariantIndex = 0;
        sidekickLoader.EquipPart(currentPart, currentVariantIndex);
        UpdateVariantDisplay();
    }

    private void OnLeftClicked()
    {
        if (string.IsNullOrEmpty(currentPart)) return;

        var part = sidekickLoader.partsCatalog[currentPart];
        currentVariantIndex--;
        if (currentVariantIndex < 0)
            currentVariantIndex = part.prefabPaths.Count - 1;

        sidekickLoader.EquipPart(currentPart, currentVariantIndex);
        UpdateVariantDisplay();
    }

    private void OnRightClicked()
    {
        if (string.IsNullOrEmpty(currentPart)) return;

        var part = sidekickLoader.partsCatalog[currentPart];
        currentVariantIndex++;
        if (currentVariantIndex >= part.prefabPaths.Count)
            currentVariantIndex = 0;

        sidekickLoader.EquipPart(currentPart, currentVariantIndex);
        UpdateVariantDisplay();
    }

    private void UpdateVariantDisplay()
    {
        variantText.text = $"Variant: {currentVariantIndex}";
    }
}
