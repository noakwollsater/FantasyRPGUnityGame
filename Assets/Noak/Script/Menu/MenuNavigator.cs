using UnityEngine;
using System.Collections.Generic;


namespace Unity.FantasyKingdom
{
    public class MenuNavigator : MonoBehaviour
    {
        [Header("NavBar")]
        [SerializeField] private GameObject NavBar;

        [Header("Standard Menus")]
        [SerializeField] private GameObject StartMenu;
        [SerializeField] private GameObject MainMenu;
        [SerializeField] private GameObject GameMenu;
        [SerializeField] private GameObject CharacterCreationMenu;
        [SerializeField] private GameObject LoadGameMenu;

        [Header("Other Menus")]
        [SerializeField] private GameObject OptionsMenu;
        [SerializeField] private GameObject LoadingScreen;
        [SerializeField] private GameObject LoadGameConfirmationMenu;
        [SerializeField] private GameObject ScreenBrightnessMenu;
        [SerializeField] private GameObject LicensAgreementMenu;

        private Stack<GameObject> menuHistory = new Stack<GameObject>();

        private GameSettingsData gameSettings;
        private const string settingsKey = "GameSettings";

        void Start()
        {
            LoadSettings();

            if (gameSettings.firstTime)
            {
                ShowMenu(ScreenBrightnessMenu);
                ShowMenu(LicensAgreementMenu);
                gameSettings.firstTime = false;
                SaveSettings();
            }
            else
            {
                PushAndShowMenu(StartMenu);
            }

            NavBar.SetActive(false); // Start with NavBar hidden
        }

        void LoadSettings()
        {
            if (ES3.KeyExists(settingsKey))
                gameSettings = ES3.Load<GameSettingsData>(settingsKey);
            else
                gameSettings = new GameSettingsData(); // First time
        }

        void SaveSettings()
        {
            ES3.Save(settingsKey, gameSettings);
        }

        void ShowMenu(GameObject menu)
        {
            menu?.SetActive(true);
        }

        void HideMenu(GameObject menu)
        {
            menu?.SetActive(false);
        }

        // 🟩 Called by button or space/enter key
        public void OnContinueFromStart()
        {
            HideMenu(StartMenu);
            PushAndShowMenu(MainMenu);
            NavBar.SetActive(true);
        }

        public void GoToGameMenu()
        {
            PushAndShowMenu(GameMenu);
        }

        public void GoToCharacterCreation()
        {
            PushAndShowMenu(LoadingScreen);
            Invoke(nameof(ShowCharacterCreation), 1f); // Simulate async loading
            NavBar.SetActive(false);
        }

        private void ShowCharacterCreation()
        {
            HideMenu(LoadingScreen);
            PushAndShowMenu(CharacterCreationMenu);
        }

        public void GoToLoadGameMenu()
        {
            PushAndShowMenu(LoadGameMenu);
        }

        public void GoToOptions()
        {
            PushAndShowMenu(OptionsMenu);
        }

        public void BackToMainMenu()
        {
            PushAndShowMenu(MainMenu);
        }

        private void HideAllMenus()
        {
            StartMenu.SetActive(false);
            MainMenu.SetActive(false);
            GameMenu.SetActive(false);
            CharacterCreationMenu.SetActive(false);
            LoadGameMenu.SetActive(false);
            OptionsMenu.SetActive(false);
            ScreenBrightnessMenu.SetActive(false);
            LicensAgreementMenu.SetActive(false);
            LoadingScreen.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (StartMenu.activeSelf)
                    OnContinueFromStart();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBack();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!OptionsMenu.activeSelf)
                    GoToOptions();
                else
                    GoBack();
            }
        }

        private GameObject GetCurrentActiveMenu()
        {
            GameObject[] allMenus = {
        StartMenu, MainMenu, GameMenu, CharacterCreationMenu,
        LoadGameMenu, OptionsMenu, ScreenBrightnessMenu, LicensAgreementMenu
    };

            foreach (var menu in allMenus)
            {
                if (menu != null && menu.activeSelf)
                    return menu;
            }

            return null;
        }

        public void GoBack()
        {
            while (menuHistory.Count > 0)
            {
                GameObject previousMenu = menuHistory.Pop();

                if (previousMenu != GetCurrentActiveMenu())
                {
                    HideAllMenus();
                    ShowMenu(previousMenu);
                    return;
                }
            }

            Debug.Log("No usable menu to go back to.");
        }

        private void PushAndShowMenu(GameObject newMenu)
        {
            GameObject currentMenu = GetCurrentActiveMenu();

            // 🧠 Only push if the current menu is not the same as the one we’re about to show
            if (currentMenu != null && currentMenu != newMenu)
                menuHistory.Push(currentMenu);

            HideAllMenus();
            ShowMenu(newMenu);
        }
    }
}
