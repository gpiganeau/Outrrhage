using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : MonoBehaviour
{
    [Header("Scene")]
    [Tooltip("Name of the Main Menu scene to load")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("References")]
    [SerializeField] private SettingsManager settingsManager;

    // ── Screens ──────────────────────────────────────────────────────────────
    private VisualElement _screenPause;
    private VisualElement _screenSettings;
    private VisualElement _screenConfirm;
    private VisualElement _root;

    // ── Settings panels ───────────────────────────────────────────────────────
    private SettingsPanelCamera   _panelCamera;
    private SettingsPanelGameplay _panelGameplay;
    private SettingsPanelVisual   _panelVisual;
    private SettingsPanelAudio    _panelAudio;

    // ── Confirm state ─────────────────────────────────────────────────────────
    private enum ConfirmType { None, MainMenu, Quit }
    private ConfirmType _pendingConfirm = ConfirmType.None;

    // ── Sidebar tabs ──────────────────────────────────────────────────────────
    private string _activeTab = "gameplay";

    private bool _isPaused = false;

    // ─────────────────────────────────────────────────────────────────────────

    private void Awake()
    {
        if (settingsManager == null)
            settingsManager = SettingsManager.Instance;

    }

    void Start()
    {
        InputManager.Instance.OnPauseEvent.AddListener(TogglePause);
    }

    private void OnEnable()
    {
        var doc = GetComponent<UIDocument>();
        _root = doc.rootVisualElement;



        // Screens
        _screenPause    = _root.Q<VisualElement>("screen-pause");
        _screenSettings = _root.Q<VisualElement>("screen-settings");
        _screenConfirm  = _root.Q<VisualElement>("screen-confirm");

        BindPauseScreen();
        BindSettingsScreen();
        BindConfirmScreen();

        ShowScreen("none");
    }

    private void OnDisable()
    {
    if (InputManager.Instance != null)
        InputManager.Instance.OnPauseEvent.RemoveListener(TogglePause);
    }



    // ═════════════════════════════════════════════════════════════════════════
    //  PAUSE / RESUME
    // ═════════════════════════════════════════════════════════════════════════

    public void TogglePause()
    {
        if (_isPaused) Resume();
        else           OpenPause();
    }

    private void OpenPause()
    {
        _isPaused = true;
        Time.timeScale = 0f;

        InputManager.Instance.PlayerInputComponent
        .SwitchCurrentActionMap(
            SettingsManager.Instance.Standards.INPUT_UI_MAP
        );

        ShowScreen("pause");
    }

    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;

        InputManager.Instance.PlayerInputComponent
        .SwitchCurrentActionMap(
            SettingsManager.Instance.Standards.INPUT_CHARACTER_MAP 
        );
        ShowScreen("none");
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  SCREEN MANAGEMENT
    // ═════════════════════════════════════════════════════════════════════════

    private void ShowScreen(string name)
    {
        SetVisible(_screenPause,    name == "pause");
        SetVisible(_screenSettings, name == "settings");
        SetVisible(_screenConfirm,  name == "confirm");
    }

    private static void SetVisible(VisualElement el, bool visible)
    {
        if (el == null) return;
        el.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  PAUSE SCREEN BINDINGS
    // ═════════════════════════════════════════════════════════════════════════

    private void BindPauseScreen()
    {
        Bind("btn-resume",    () => Resume());
        Bind("btn-settings",  () => ShowScreen("settings"));
        Bind("btn-mainmenu",  () => AskConfirm(ConfirmType.MainMenu));
        Bind("btn-quit",      () => AskConfirm(ConfirmType.Quit));
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  SETTINGS SCREEN BINDINGS
    // ═════════════════════════════════════════════════════════════════════════

    private void BindSettingsScreen()
    {
        // Back button
        Bind("btn-back-settings", () => ShowScreen("pause"));

        // Apply / Reset
        Bind("btn-apply",  ApplySettings);
        Bind("btn-reset",  ResetSettings);

        // Sidebar tabs
        string[] tabs = { "camera", "gameplay", "visual", "audio" };
        foreach (var tab in tabs)
        {
            string captured = tab;
            Bind($"tab-{tab}", () => SwitchTab(captured));
        }

        // Build sub-panels
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

        // Update header label
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

    // ═════════════════════════════════════════════════════════════════════════
    //  CONFIRM SCREEN BINDINGS
    // ═════════════════════════════════════════════════════════════════════════

    private void BindConfirmScreen()
    {
        Bind("btn-confirm-cancel",  () => ShowScreen("pause"));
        Bind("btn-confirm-ok",      ExecuteConfirm);
    }

    private void AskConfirm(ConfirmType type)
    {
        _pendingConfirm = type;

        var titleLabel = _root.Q<Label>("confirm-title");
        var subLabel   = _root.Q<Label>("confirm-sub");

        if (titleLabel != null)
            titleLabel.text = type == ConfirmType.Quit
                ? "Quitter le jeu ?"
                : "Retourner au menu principal ?";

        if (subLabel != null)
            subLabel.text = type == ConfirmType.Quit
                ? "Toute progression non sauvegardée sera perdue."
                : "Votre progression sera sauvegardée.";

        ShowScreen("confirm");
    }

    private void ExecuteConfirm()
    {
        Time.timeScale = 1f;
        switch (_pendingConfirm)
        {
            case ConfirmType.MainMenu:
                SceneManager.LoadScene(mainMenuSceneName);
                break;
            case ConfirmType.Quit:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    // ═════════════════════════════════════════════════════════════════════════
    //  HELPERS
    // ═════════════════════════════════════════════════════════════════════════

    private void Bind(string name, System.Action action)
    {
        var btn = _root.Q<Button>(name);
        if (btn != null) btn.clicked += action;
        else Debug.LogWarning($"[PauseMenu] Button not found: {name}");
    }
}
