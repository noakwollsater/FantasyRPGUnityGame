using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;


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
        [SerializeField] private GameObject QuitGameMenu;

        [Header("Loading Screen")]
        [SerializeField] private Slider loadingSlider;

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
                // Check for return from Character Creation
                if (PlayerPrefs.HasKey("ReturnToMainMenu") && PlayerPrefs.GetInt("ReturnToMainMenu") == 1)
                {
                    PlayerPrefs.DeleteKey("ReturnToMainMenu");
                    PushAndShowMenu(MainMenu);  // 👉 Skip StartMenu
                    NavBar.SetActive(true);
                }
                else
                {
                    PushAndShowMenu(StartMenu); // 👈 Default start
                    NavBar.SetActive(false);
                }
            }
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
            if (GetCurrentActiveMenu() != null)
                menuHistory.Push(GetCurrentActiveMenu());

            HideAllMenus();
            StartCoroutine(LoadCharacterCreationAsync());
            NavBar.SetActive(false);
        }
        private IEnumerator LoadCharacterCreationAsync()
        {
            ShowMenu(LoadingScreen);
            NavBar.SetActive(false);

            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("CharacterCreation");

            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                // 🔁 Update your loading slider here!
                loadingSlider.value = progress;

                // Unity loads up to 0.9, then waits for activation
                if (operation.progress >= 0.9f)
                {
                    // Optional: Add delay, or wait for user input before continuing
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
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

        public void GoToAreYouSureYouWantToQuit()
        {
            ShowMenu(QuitGameMenu);
        }

        public void CloseQuitPanel()
        {
            HideMenu(QuitGameMenu);
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
            QuitGameMenu.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                if (StartMenu.activeSelf)
                    OnContinueFromStart();
                else if (QuitGameMenu.activeSelf)
                    Application.Quit();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (QuitGameMenu.activeSelf)
                {
                    CloseQuitPanel();
                }
                else
                {
                    GoBack(); // 🔙 Normal back behavior
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab) && !StartMenu.activeSelf)
            {
                if (!OptionsMenu.activeSelf)
                    GoToOptions();
                else
                    GoBack();
            }
            if (Input.GetKeyDown(KeyCode.CapsLock))
            {
                GoToAreYouSureYouWantToQuit();
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

                    // ✅ Re-enable NavBar if needed
                    if (previousMenu == MainMenu || previousMenu == GameMenu || previousMenu == LoadGameMenu)
                        NavBar.SetActive(true);
                    else
                        NavBar.SetActive(false);

                    return;
                }
            }

            Debug.Log("No usable menu to go back to.");
        }

        private void PushAndShowMenu(GameObject newMenu)
        {
            // 👇 Capture the current menu BEFORE hiding everything
            GameObject currentMenu = GetCurrentActiveMenu();

            if (currentMenu != null && currentMenu != newMenu)
            {
                menuHistory.Push(currentMenu);
            }

            HideAllMenus();
            ShowMenu(newMenu);
        }
    }
}
