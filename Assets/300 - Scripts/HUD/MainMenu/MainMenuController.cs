using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Exact name of the gameplay scene")]
    [SerializeField] private string gameSceneName = "RoomScene";

    [Header("References")]
    [SerializeField] private SettingsManager settingsManager;

    // ── Screens ───────────────────────────────────────────────────────────
    private VisualElement _root;
    private VisualElement _screenMain;
    private VisualElement _screenSettings;
    private VisualElement _screenTeam;
    private VisualElement _screenConfirm;

    // ── Settings panels (reuse from PauseMenu) ────────────────────────────
    private SettingsPanelCamera   _panelCamera;
    private SettingsPanelGameplay _panelGameplay;
    private SettingsPanelVisual   _panelVisual;
    private SettingsPanelAudio    _panelAudio;

    private string _activeTab = "gameplay";

    // ─────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (settingsManager == null)
            settingsManager = SettingsManager.Instance;
    }

    private void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        _root = doc.rootVisualElement;

        _screenMain     = _root.Q("screen-main");
        _screenSettings = _root.Q("screen-settings");
        _screenTeam     = _root.Q("screen-team");
        _screenConfirm  = _root.Q("screen-confirm");

        BindMainScreen();
        BindSettingsScreen();
        BindTeamScreen();
        BindConfirmScreen();

        // Register input
        if (InputManager.Instance != null)
            InputManager.Instance.OnPauseEvent.AddListener(OnEscapePressed);

        ShowScreen("main");

        // Animate title on load
        AnimateTitleIn();
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnPauseEvent.RemoveListener(OnEscapePressed);
    }

    // ═════════════════════════════════════════════════════════════════════
    //  ESCAPE / BACK HANDLING
    // ═════════════════════════════════════════════════════════════════════

    private void OnEscapePressed()
    {
        if (_screenSettings.style.display == DisplayStyle.Flex)
            ShowScreen("main");
        else if (_screenTeam.style.display == DisplayStyle.Flex)
            ShowScreen("main");
        else if (_screenConfirm.style.display == DisplayStyle.Flex)
            ShowScreen("main");
    }

    // ═════════════════════════════════════════════════════════════════════
    //  SCREEN MANAGEMENT
    // ═════════════════════════════════════════════════════════════════════

    private void ShowScreen(string name)
    {
        SetVisible(_screenMain,     name == "main");
        SetVisible(_screenSettings, name == "settings");
        SetVisible(_screenTeam,     name == "team");
        SetVisible(_screenConfirm,  name == "confirm");
    }

    private static void SetVisible(VisualElement el, bool visible)
    {
        if (el == null) return;
        el.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    // ═════════════════════════════════════════════════════════════════════
    //  MAIN SCREEN
    // ═════════════════════════════════════════════════════════════════════

    private void BindMainScreen()
    {
        Bind("btn-play",     StartGame);
        Bind("btn-settings", () => ShowScreen("settings"));
        Bind("btn-team",     () => ShowScreen("team"));
        Bind("btn-quit",     () => ShowScreen("confirm"));
    }

    private void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // ═════════════════════════════════════════════════════════════════════
    //  SETTINGS SCREEN  (same logic as PauseMenuController)
    // ═════════════════════════════════════════════════════════════════════

    private void BindSettingsScreen()
    {
        Bind("btn-back-settings", () => ShowScreen("main"));
        Bind("btn-apply",  ApplySettings);
        Bind("btn-reset",  ResetSettings);

        string[] tabs = { "camera", "gameplay", "visual", "audio" };
        foreach (var tab in tabs)
        {
            string captured = tab;
            Bind($"tab-{tab}", () => SwitchTab(captured));
        }

        _panelCamera   = new SettingsPanelCamera  (_root, settingsManager);
        _panelGameplay = new SettingsPanelGameplay (_root, settingsManager);
        _panelVisual   = new SettingsPanelVisual   (_root, settingsManager);
        _panelAudio    = new SettingsPanelAudio    (_root, settingsManager);

        SwitchTab("gameplay");
    }

    private void SwitchTab(string tab)
    {
        _activeTab = tab;
        string[] tabs = { "camera", "gameplay", "visual", "audio" };
        foreach (var t in tabs)
        {
            var btn   = _root.Q<Button>($"tab-{t}");
            var panel = _root.Q<VisualElement>($"panel-{t}");
            bool active = t == tab;
            btn?.EnableInClassList("tab--active", active);
            SetVisible(panel, active);
        }

        var header = _root.Q<Label>("settings-category-label");
        if (header != null)
            header.text = System.Globalization.CultureInfo.CurrentCulture
                          .TextInfo.ToTitleCase(tab);
    }

    private void ApplySettings()
    {
        _panelCamera  ?.Apply();
        _panelGameplay?.Apply();
        _panelVisual  ?.Apply();
        _panelAudio   ?.Apply();
    }

    private void ResetSettings()
    {
        _panelCamera  ?.Reset();
        _panelGameplay?.Reset();
        _panelVisual  ?.Reset();
        _panelAudio   ?.Reset();
    }

    // ═════════════════════════════════════════════════════════════════════
    //  TEAM SCREEN
    // ═════════════════════════════════════════════════════════════════════

    private void BindTeamScreen()
    {
        Bind("btn-back-team", () => ShowScreen("main"));
    }

    // ═════════════════════════════════════════════════════════════════════
    //  CONFIRM QUIT SCREEN
    // ═════════════════════════════════════════════════════════════════════

    private void BindConfirmScreen()
    {
        Bind("btn-confirm-cancel", () => ShowScreen("main"));
        Bind("btn-confirm-ok",     QuitGame);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ═════════════════════════════════════════════════════════════════════
    //  TITLE ANIMATION
    // ═════════════════════════════════════════════════════════════════════

    private void AnimateTitleIn()
    {
        var title = _root.Q<Label>("main-title");
        if (title == null) return;

        title.AddToClassList("title--hidden");

        // Remove hidden class after 1 frame to trigger transition
        title.schedule.Execute(() =>
        {
            title.RemoveFromClassList("title--hidden");
            title.AddToClassList("title--visible");
        }).StartingIn(50);
    }

    // ═════════════════════════════════════════════════════════════════════
    //  HELPERS
    // ═════════════════════════════════════════════════════════════════════

    private void Bind(string name, System.Action action)
    {
        var btn = _root.Q<Button>(name);
        if (btn != null) btn.clicked += action;
        else Debug.LogWarning($"[MainMenu] Button not found: {name}");
    }
}
