using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    public TriangleStatController statController;
    public TMP_Text muscleText;
    public TMP_Text fatText;
    public TMP_Text dexterityText;

    private void Update()
    {
        muscleText.text = "Muscle: " + statController.muscle;
        fatText.text = "Fat: " + statController.fat;
        dexterityText.text = "Dexterity: " + statController.dexterity;
    }
}
