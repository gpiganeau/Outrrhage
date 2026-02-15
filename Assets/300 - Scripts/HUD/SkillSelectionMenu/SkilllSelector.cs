using UnityEngine;

public class SkillSelector : MonoBehaviour
{
    public static SkillSelector Instance { get; private set; }
    
    [SerializeField] private SkillSelectionUIManager _skillSelectionUI;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void OpenMenuForPlayer(SkillsController playerSkillsController)
    {
        _skillSelectionUI.SetTargetController(playerSkillsController);
        _skillSelectionUI.OpenMenu();
    }
    
    public void ToggleMenu()
    {
        _skillSelectionUI.ToggleMenu();
    }
}