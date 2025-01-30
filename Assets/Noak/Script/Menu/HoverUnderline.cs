using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverUnderline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image underline; // Drag din streck-sprite h�r i Inspector.

    private void Start()
    {
        // Se till att strecket �r osynligt n�r spelet startar
        if (underline != null)
        {
            underline.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Visa strecket n�r musen hovrar �ver knappen
        if (underline != null)
        {
            underline.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // D�lj strecket n�r musen l�mnar knappen
        if (underline != null)
        {
            underline.enabled = false;
        }
    }
}
