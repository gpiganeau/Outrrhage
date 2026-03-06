using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Fields
    public static GameManager Instance;

    [HideInInspector] public CharacterComponent Riel;

    [Header("Core Prefabs References")]
    public CharacterComponent _rielPrefab;

    [Header("Managers References")]
    [SerializeField] CameraController _cameraController;

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
            _draftUI.OnDraftConfirmed += OnDraftConfirmed;
            _draftUI.Show();

        } else
        {
            _draftUI.Hide();
            RunSystemController.Instance.StartRun(null);
        }

        InputManager.Instance.OnReloadGameEvent.AddListener(ReloadCurrentScene);

    }


    void OnDisable()
    {
        InputManager.Instance.OnReloadGameEvent.RemoveListener(ReloadCurrentScene);
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
        Logger.LogError(Logger.LogCategory.Core, "Demo End");
    }
}
