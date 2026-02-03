using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public CharacterComponent Riel;

    [Header("Core Prefabs References")]
    public CharacterComponent _rielPrefab;
    public Level _startLevelPrefab;

    [Header("Managers References")]
    [SerializeField] CameraController _cameraController;

    [Header("Readonly References for Debug")]
    public Level _currentLevel;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    public void Start(){

        Sequence spawnSeq = DOTween.Sequence();
        
        spawnSeq.AppendCallback( () => _currentLevel = Instantiate(_startLevelPrefab, Vector3.zero, Quaternion.identity));
        spawnSeq.AppendInterval(0.5F);
        spawnSeq.AppendCallback( () =>
        {
            RespawnPoint spawnPoint = _currentLevel.GetSpawnPoint();
            var riel = Instantiate(_rielPrefab, spawnPoint.transform.position, Quaternion.identity) as CharacterComponent;
            Riel = riel;
            _cameraController.SetTarget(Riel.transform);
            _cameraController.transform.position = spawnPoint.transform.position + new Vector3(0, 100, 0);
            riel.PlayerCameraController = _cameraController;
        });

        spawnSeq.Play();
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
