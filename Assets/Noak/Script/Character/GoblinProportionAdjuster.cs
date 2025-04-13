using Sirenix.OdinInspector.Editor.Internal;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GoblinProportionAdjuster : MonoBehaviour
{
    private readonly string saveKey = "MyCharacter";

    [Header("Overall Scale")]
    public float heightScale = 0.85f;

    [Header("Head Proportion")]
    public Transform headBone;
    public float headScale = 1.4f;

    [Header("Goblin Scale (optional)")]
    public Transform leftArm;
    public Transform rightArm;
    public Transform leftLeg;
    public Transform rightLeg;
    public float limbScale = 0.95f;

    [SerializeField, Tooltip("Hur mycket armarna dras ut från kroppen (X-axeln).")]
    private float armSpreadOffset = 0.05f; // Justera efter smak


    private CharacterSaveData data;

    private string selectedRace;

    void Start()
    {
        string fileName = $"CharacterSave_{PlayerPrefs.GetString("SavedCharacterName", "Default")}.es3";
        var settings = new ES3Settings(fileName);

        if (!ES3.FileExists(fileName) || !ES3.KeyExists(saveKey, settings))
        {
            Debug.LogWarning("⚠️ Ingen sparad karaktär hittades.");
            return;
        }

        data = ES3.Load<CharacterSaveData>(saveKey, settings);
        Debug.Log("✅ Laddade sparad karaktär!");

        selectedRace = data.race.ToString();

        if(selectedRace == "Goblin")
        {
            SetGoblinSize();
        }
    }

    private void SetGoblinSize()
    {
        // Shrink whole body
        transform.localScale = Vector3.one * heightScale;

        // Enlarge head
        if (headBone != null)
            headBone.localScale = Vector3.one * headScale;

        // Shrink limbs
        if (leftArm != null) leftArm.localScale = Vector3.one * limbScale;
        if (rightArm != null) rightArm.localScale = Vector3.one * limbScale;
        if (leftLeg != null) leftLeg.localScale = Vector3.one * limbScale;
        if (rightLeg != null) rightLeg.localScale = Vector3.one * limbScale;

        if(data.fat > 60)
        {
            // Anpassa armar – skala ut i X-led
            if (leftArm != null)
                leftArm.localScale = new Vector3(1.1f, limbScale, limbScale);

            if (rightArm != null)
                rightArm.localScale = new Vector3(1.1f, limbScale, limbScale);

        }
    }
}
