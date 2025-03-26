using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class MenuNavigator : MonoBehaviour
    {
        [Header("Standard Menus")]
        [SerializeField] private GameObject StartMenu;
        [SerializeField] private GameObject MainMenu;
        [SerializeField] private GameObject GameMenu;
        [SerializeField] private GameObject CharacterCreationMenu;
        [SerializeField] private GameObject LoadGameMenu;

        [Header("Character Creation Menu")]
        [SerializeField] private GameObject CharacterCreator;

        [Header("Other Menus")]
        [SerializeField] private GameObject OptionsMenu;
        [SerializeField] private GameObject LoadingScreen;
        [SerializeField] private GameObject LoadGameConfirmationMenu;
        [SerializeField] private GameObject ScreenBrightnessMenu;
        [SerializeField] private GameObject LicensAgreementMenu;
        //[SerializeField] private GameObject CreditsMenu;
    }
}
