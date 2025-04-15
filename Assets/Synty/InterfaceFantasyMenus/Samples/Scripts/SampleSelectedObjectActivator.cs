using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace Synty.Interface.FantasyMenus.Samples
{
    /// <summary>
    /// Aktivator som visar ett tillh�rande objekt n�r dess knapp klickas,
    /// och d�ljer alla andras.
    /// </summary>
    public class SampleSelectedObjectActivator : MonoBehaviour
    {
        [Header("References")]
        public Selectable selectable;
        public GameObject isOnObject;

        private void Awake()
        {
            // F�rs�k auto-h�mta om inte satt via Inspector
            if (selectable == null)
                selectable = GetComponent<Selectable>();
        }

        private void OnEnable()
        {
            if (selectable is Button btn)
                btn.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            if (selectable is Button btn)
                btn.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            // Aktivera v�rt objekt
            if (isOnObject != null)
                isOnObject.SetActive(true);

            // Inaktivera alla andras
            var allActivators = FindObjectsOfType<SampleSelectedObjectActivator>();

            foreach (var activator in allActivators)
            {
                if (activator != this && activator.isOnObject != null)
                {
                    activator.isOnObject.SetActive(false);
                }
            }

            // Markera denna som vald i EventSystem
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }
    }
}
