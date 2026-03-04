using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    #region Fields
    public static GameManager Instance;

    public enum GameMode { Forest, Rooms }

    public GameMode CurrentGameMode = GameMode.Rooms;

    [HideInInspector] public CharacterComponent Riel;

    [Header("Core Prefabs References")]
    public CharacterComponent _rielPrefab;
    public Level _startLevelPrefab;

    public List<GameRoom> _roomsList;

    [Header("Managers References")]
    [SerializeField] CameraController _cameraController;

    [Header("Readonly References for Debug")]
    public Level _currentLevel;
    [SerializeField] private RunSystemController runController;

    public UnityEvent OnGameStart;
    public CameraController CameraController => _cameraController;

    private static bool _gameOver = false;
    public static bool GameOver { get => _gameOver; set => _gameOver = value;}

    [Header("Run UI Draft")]
    [SerializeField] private RunDraftUI _draftUI;

    #endregion

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public void Start()
    {

        DOTween.SetTweensCapacity(500, 50);

        if (SettingsManager.Instance.GameplaySettings.OpenSkillSelectorOnRunStart)
        {
            Logger.Core("Game Manager Start : Register Draft & Show UI");
            _draftUI.OnDraftConfirmed += OnDraftConfirmed;
            _draftUI.Show();

        } else
        {
            _draftUI.Hide();
            RunSystemController.Instance.StartRun(null);
        }
    }

    public void OnRielReady()
    {
        // -- Event ?
        // -- Apply skills ?
    }

    private void OnDraftConfirmed(List<SkillData> skills)
    {
        SkillsFromDraft = skills;
        RunSystemController.Instance.StartRun(SkillsFromDraft);
    }

    public static List<SkillData> SkillsFromDraft;

    public void SpawnRiel(Vector3 rielSpawnPos, List<SkillData> skills)
    {
        var riel = Instantiate(_rielPrefab, rielSpawnPos, Quaternion.identity);
        Riel = riel;
        _cameraController.SetTarget(Riel.transform);
        _cameraController.transform.position = riel.transform.position.WithY(100);
        riel.PlayerCameraController = _cameraController;
        EntityManager.Instance.Riel = riel;
        OnGameStart.Invoke();
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TriggerDemoEnd()
    {
        Logger.Core("Demo End");
    }
}
