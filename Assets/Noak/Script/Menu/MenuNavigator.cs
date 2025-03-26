using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class MenuNavigator : MonoBehaviour
    {
        [Header("Standard Menus")]
        public GameObject StartMenu;
        public GameObject MainMenu;
        public GameObject GameMenu;
        public GameObject CharacterCreationMenu;
        public GameObject LoadGameMenu;

        [Header("Character Creation Menu")]
        public GameObject CharacterCreator;

        [Header("Other Menus")]
        public GameObject OptionsMenu;
        public GameObject LoadingScreen;
        public GameObject LoadGameConfirmationMenu;
        public GameObject ScreenBrightnessMenu;
        public GameObject LicensAgreementMenu;
        //public GameObject CreditsMenu;
    }
}
