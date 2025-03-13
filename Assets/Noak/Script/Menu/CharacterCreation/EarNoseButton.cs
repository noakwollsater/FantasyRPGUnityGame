using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EarNoseButton : CharacterCreation, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image earNoseIcon;
    [SerializeField] private Sprite earIcon;
    [SerializeField] private Sprite noseIcon;

    private void Start()
    {
        UpdateIcon();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        earNoseIcon.sprite = isNose ? earIcon : noseIcon;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateIcon();
    }

    public void OnButtonClick()
    {
        isNose = !isNose;
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        earNoseIcon.sprite = isNose ? noseIcon : earIcon;
    }
}
