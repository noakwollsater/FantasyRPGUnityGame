using Synty.Interface.FantasyMenus.Samples;
using Unity.FantasyKingdom;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveListPopulator : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject saveCardPrefab;
    [SerializeField] private SaveLoader saveLoader;

    [SerializeField] private Transform detailedContentParent;
    [SerializeField] private GameObject detailedContent;
    [SerializeField] private GameObject detailedCardPrefab;

    [SerializeField] private GameObject FirstSpacer;
    [SerializeField] private GameObject SecondSpacer;

    SampleSelectedObjectActivator sampleSelectedObjectActivator;

    private void Start()
    {
        PopulateList();
    }

    private void PopulateList()
    {
        var saves = saveLoader.LoadAllSaves();

        if (FirstSpacer == null || SecondSpacer == null)
        {
            Debug.LogError("❌ Spacers are not assigned!");
            return;
        }

        int insertIndex = contentParent.GetSiblingIndex() + FirstSpacer.transform.GetSiblingIndex() + 1;

        bool isFirst = true;

        foreach (var data in saves)
        {
            GameObject card = Instantiate(saveCardPrefab);
            card.transform.SetParent(contentParent, false);
            card.transform.SetSiblingIndex(SecondSpacer.transform.GetSiblingIndex());

            // ...
            GameObject detailedCard = Instantiate(detailedCardPrefab);
            detailedCard.transform.SetParent(detailedContentParent, false);
            detailedCard.SetActive(false); // Dölj först
            Button btn = card.GetComponent<Button>();
            // 🟢 Lägg activatorn på kortet, inte på detailedContent!
            sampleSelectedObjectActivator = card.AddComponent<SampleSelectedObjectActivator>();
            sampleSelectedObjectActivator.selectable = btn;
            sampleSelectedObjectActivator.isOnObject = detailedCard;

            sampleSelectedObjectActivator.selectable = btn;
            sampleSelectedObjectActivator.isOnObject = detailedCard;

            LoadGameUI cardUI = card.GetComponent<LoadGameUI>();
            LoadDetailedUI detailedUI = detailedCard.GetComponent<LoadDetailedUI>();
            detailedUI.SetData(data);
            cardUI.SetData(data);

            Button detailbtn = detailedCard.GetComponentInChildren<Button>();
            LoadGame loadGameScript = detailedCard.GetComponent<LoadGame>();

            if (loadGameScript != null && detailbtn != null)
            {
                loadGameScript.saveData = data;
                detailbtn.onClick.AddListener(loadGameScript.OnClickselectGameBtn);
                Debug.Log($"✅ Listener kopplad till: {detailbtn.gameObject.name}, med script på {loadGameScript.gameObject.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ LoadGame script or Button missing on card/detailedCard.");
            }

            // 🎯 Förvalt val - visa första kortet direkt
            if (isFirst)
            {
                detailedCard.SetActive(true);
                EventSystem.current.SetSelectedGameObject(btn.gameObject); // Så att SampleSelectedObjectActivator reagerar
                isFirst = false;
            }
        }
    }
}
