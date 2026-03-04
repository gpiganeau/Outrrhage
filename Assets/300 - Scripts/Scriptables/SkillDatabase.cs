using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "Scriptable Objects/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    [SerializeField] private string _skillsFolderPath = "Assets/ScriptableObjects/Skills";
    [SerializeField] private List<SkillData> _allSkills = new List<SkillData>();
    
    public List<SkillData> AllSkills => _allSkills;
    
    public SkillData GetSkillByName(string skillName)
    {
        return _allSkills.Find(s => s.name == skillName);
    }
    
#if UNITY_EDITOR
    [ContextMenu("Refresh Skill List")]
    public void RefreshSkillList()
    {
        _allSkills.Clear();
        
        string[] guids = AssetDatabase.FindAssets("t:SkillData", new[] { _skillsFolderPath });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SkillData skill = AssetDatabase.LoadAssetAtPath<SkillData>(path);
            
            if (skill != null && !_allSkills.Contains(skill))
            {
                _allSkills.Add(skill);
            }
        }
        
        _allSkills.Sort((a, b) => string.Compare(a.name, b.name));
        
        EditorUtility.SetDirty(this);
        
        //Debug.Log($"SkillDatabase refreshed: {_allSkills.Count} skills found in {_skillsFolderPath}");
    }
    
    private void OnValidate()
    {
        RefreshSkillList();
    }
#endif
}