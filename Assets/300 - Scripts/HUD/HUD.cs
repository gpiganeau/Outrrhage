using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{

    [Header("References")]
    [SerializeField] SkillBar _skillBar;
    [SerializeField] SkillsController _skillsController;

    HUD Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }
    void OnEnable()
    {
        if (_skillsController != null) _skillsController.OnSkillsInitialized += OnSkillsChanged;
    }

    void OnDisable()
    {
        if (_skillsController != null) _skillsController.OnSkillsInitialized -= OnSkillsChanged;
    }


    public void Initalize()
    {
    }

    private void OnSkillsChanged(List<SkillStrategy> strategies)
    {
        _skillBar.Init(strategies);
    }
}
