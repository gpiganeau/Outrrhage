#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SkillDatabase))]
public class SkillDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        SkillDatabase database = (SkillDatabase)target;
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🔄 Refresh Skill List", GUILayout.Height(40)))
        {
            database.RefreshSkillList();
        }
        
        GUILayout.Space(5);
        GUILayout.Label($"Total Skills: {database.AllSkills.Count}", EditorStyles.boldLabel);
    }
}
#endif