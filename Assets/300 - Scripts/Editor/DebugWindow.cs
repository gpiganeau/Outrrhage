using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class DebugWindow : EditorWindow
{
    private Vector2 scrollPos;
    private static List<DebugAction> actions = new List<DebugAction>();

    [MenuItem("Tools/Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<DebugWindow>("Debug");
    }

    private void OnEnable()
    {
        RegisterActions();
    }


    // -- Register new Actions Here ! @Gregoire 
    private void RegisterActions()
    {
        actions.Clear();

        AddAction("Restart Level", () => 
        {
            GameManager.Instance.ReloadCurrentScene();
        });
    }


    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var action in actions)
        {
            if (GUILayout.Button(action.label, GUILayout.Height(30)))
            {
                action.callback?.Invoke();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    public static void AddAction(string label, Action callback)
    {
        actions.Add(new DebugAction { label = label, callback = callback });
    }

    private class DebugAction
    {
        public string label;
        public Action callback;
    }
}