using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClassSkills : MonoBehaviour
{
    [SerializeField] private ClassDatabase classDatabase;
    [SerializeField] private Sprite[] skillIcons;

    [SerializeField] private TMP_Text skill1Name;
    [SerializeField] private TMP_Text description;

    [SerializeField] private Button skill1;
    [SerializeField] private Button skill2;
    [SerializeField] private Button skill3;

    public string selectedClass;

    private void OnEnable()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        selectedClass = ClassSelectionUI.SelectedClass; 
    }
}
