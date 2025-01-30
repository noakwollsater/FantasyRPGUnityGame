using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverUnderline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image underline; // Drag din streck-sprite här i Inspector.

    private void Start()
    {
        // Se till att strecket är osynligt när spelet startar
        if (underline != null)
        {
            underline.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Visa strecket när musen hovrar över knappen
        if (underline != null)
        {
            underline.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Dölj strecket när musen lämnar knappen
        if (underline != null)
        {
            underline.enabled = false;
        }
    }
}
