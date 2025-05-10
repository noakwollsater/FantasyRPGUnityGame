using Mightland.Scripts.SK;
using UnityEngine;

public class ChangeWarderobeParts : MonoBehaviour
{
    SidekickConfigurator sidekickConfigurator;

    private void OnEnable()
    {
        GameObject character = GameObject.FindGameObjectWithTag("Player");
        GameObject child = character.transform.Find("Player").gameObject;
        sidekickConfigurator = child.GetComponent<SidekickConfigurator>();
        if (sidekickConfigurator == null)
        {
            Debug.LogError("Sidekick Configurator not found on the player.");
        }
    }

    public void ForwardPart(string partGroup)
    {
        ChangePart(partGroup, true);
    }

    public void BackwardPart(string partGroup)
    {
        ChangePart(partGroup, false);
    }

    private void ChangePart(string partGroup, bool forward)
    {
        if (sidekickConfigurator == null)
        {
            Debug.LogError("SidekickConfigurator is not assigned.");
            return;
        }

        int partIndex = sidekickConfigurator.partGroups.IndexOf(partGroup);
        if (partIndex == -1)
        {
            Debug.LogWarning($"Part group '{partGroup}' not found.");
            return;
        }

        int activeIndex = sidekickConfigurator.meshPartsActive[partIndex];
        int count = sidekickConfigurator.meshPartsList[partIndex].items.Count;

        // Turn off the current active mesh
        if (sidekickConfigurator.meshPartsList[partIndex].items[activeIndex]?.meshTransform)
        {
            sidekickConfigurator.meshPartsList[partIndex].items[activeIndex].meshTransform.gameObject.SetActive(false);
        }

        // Update index
        if (forward)
        {
            activeIndex = (activeIndex + 1) % count;
        }
        else
        {
            activeIndex = (activeIndex - 1 + count) % count; // ensure it wraps around correctly
        }

        // Turn on the new active mesh
        if (sidekickConfigurator.meshPartsList[partIndex].items[activeIndex]?.meshTransform)
        {
            sidekickConfigurator.meshPartsList[partIndex].items[activeIndex].meshTransform.gameObject.SetActive(true);
        }

        sidekickConfigurator.meshPartsActive[partIndex] = activeIndex;

        sidekickConfigurator.ApplyBlendShapes();
    }

    //All keywords for the parts
    //"ABAC", "AEBL", "AEBR", "AFAC", "AHED", "AHPF", "AHPL", "AHPR", 
    //"AKNL", "AKNR", "ALWL", "ALWR", "APHB", "ASHL", "ASHR", "AUPL", "AUPR",
    //"EARL", "EARR", "EBRL", "EBRR", "EYEL", "EYER", "FCHR", "FOTL", "FOTR",
    //"Hair", "Head", "HIPS", "HNDL", "HNDR", "LEGL", "LEGR", "NOSE", "TETH", "TONG", "TORS"

    // Example functions for UI buttons
    public void ChangeBackAttachForward() => ForwardPart("ABAC");
    public void ChangeBackAttachBackward() => BackwardPart("ABAC");

}
