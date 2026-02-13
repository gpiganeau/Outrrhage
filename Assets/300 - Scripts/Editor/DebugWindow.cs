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
        GetWindow<DebugWindow>("Cheat Window");
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
            if (Application.isPlaying == false) return;
            GameManager.Instance.ReloadCurrentScene();
        });

        AddAction("Get Max Blood", () => 
        {
            if (Application.isPlaying == false) return;
            CharacterComponent.Blood.Regain(1000);
        });

        AddAction("Kill Riel", () => {
            if (Application.isPlaying == false) return;
            var riel = GameManager.Instance.Riel;
            var rielDC = riel.GetComponent<DamageController>();
            rielDC.Damage(1000,  riel.transform.position, Team.Neutral);
        });

        AddAction("Kill Enemy & Stop Spawn", () =>
        {
            if (Application.isPlaying == false) return;
            var entityManager = FindFirstObjectByType<EntityManager>();

            entityManager.StopInfiniteSpawning();

            foreach (var bot in entityManager.Bots)
            {
                if (bot != null)
                {
                    bot.ForceKill();
                }
            }
        });
        

        AddAction($"Toggle Invincibility", () => {
            if (Application.isPlaying == false) return;
            var riel = GameManager.Instance.Riel;
            var dc = riel.GetComponent<DamageController>();
            dc.IsInvincible = !dc.IsInvincible;
        });

        AddAction($"Toggle Slow Motion", () => {
            if (Application.isPlaying == false) return;
            Time.timeScale = Time.timeScale > 0.5f ? 0.2f : 1f;
        });

        AddAction($"Toggle Juicer", () => {
            if (Application.isPlaying == false) return;
            SettingsManager.Instance.VisualSettings.EnableJuicer = !SettingsManager.Instance.VisualSettings.EnableJuicer;
        });


// -- Editor Actions -- (Non Play Mode)

        AddAction("Toggle HUD", () => {
            if (Application.isPlaying == true) return;
            var canvas = FindAnyObjectByType<HUD>().GetComponent<Canvas>();
            canvas.enabled = !canvas.enabled;
        });

        AddAction("Preview Full Skill FX", () => {
            if (Application.isPlaying == true) return;

            var riel = GameManager.Instance.Riel;
            var previewer = FindFirstObjectByType<SkillDataPreviewer>();
            if (previewer != null)
            {
                previewer.PreviewSkill(riel.transform.position);
            }
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