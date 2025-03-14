using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EarNoseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button earNoseButton;
    [SerializeField] private Image earNoseIcon;
    [SerializeField] private Sprite[] Icons;

    private CharacterCreation characterCreation; // Reference to the CharacterCreation instance

    private void Start()
    {
        characterCreation = FindObjectOfType<CharacterCreation>(); // Get reference to CharacterCreation

        if (characterCreation == null)
        {
            Debug.LogError("EarNoseButton: CharacterCreation not found in the scene.");
            return;
        }

        earNoseButton.onClick.AddListener(OnButtonClick);
        UpdateIcon(); // Ensure the correct icon is set on start
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        earNoseIcon.sprite = characterCreation.isNose ? Icons[0] : Icons[1];
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateIcon();
    }

    public void OnButtonClick()
    {
        Debug.Log("EarNoseButton: Button clicked.");

        characterCreation.isNose = !characterCreation.isNose; // Update isNose in CharacterCreation

        Debug.Log($"EarNoseButton: isNose is now {characterCreation.isNose}");

        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (Icons.Length >= 2)
        {
            earNoseIcon.sprite = !characterCreation.isNose ? Icons[0] : Icons[1];
            Debug.Log($"UpdateIcon: isNose={characterCreation.isNose}, Setting sprite to {(characterCreation.isNose ? "Icons[1]" : "Icons[0]")}");
        }
        else
        {
            Debug.LogError("EarNoseButton: Not enough icons provided.");
        }
    }
}
