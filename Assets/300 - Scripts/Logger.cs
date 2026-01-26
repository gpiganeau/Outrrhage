using UnityEngine;
using System;

public static class Logger
{
    [Flags]
    public enum LogCategory
    {
        None = 0,
        Combat = 1 << 0,
        Narration = 1 << 1,
        Enviro = 1 << 2,
        Core = 1 << 3,
        Audio = 1 << 4,
        All = ~0
    }

    public static bool EnableLogs = true;
    public static bool ErrorsOnly = false;
    public static LogCategory ActiveCategories = LogCategory.All;

    private static readonly string CombatColor = "#dd9324";
    private static readonly string NarrationColor = "#3c9993";
    private static readonly string EnviroColor = "#8248df";
    private static readonly string CoreColor = "#bd1bd3";
    private static readonly string AudioColor = "#ddd34b";

    public static void Log(LogCategory category, string message)
    {
        if (!ShouldLog(category, false)) return;
        
        string coloredMessage = FormatMessage(category, message);
        Debug.Log(coloredMessage);
    }

    public static void LogWarning(LogCategory category, string message)
    {
        if (!ShouldLog(category, false)) return;
        
        string coloredMessage = FormatMessage(category, message);
        Debug.LogWarning(coloredMessage);
    }

    public static void LogError(LogCategory category, string message)
    {
        if (!ShouldLog(category, true)) return;
        
        string coloredMessage = FormatMessage(category, message);
        Debug.LogError(coloredMessage);
    }

    private static bool ShouldLog(LogCategory category, bool isError)
    {
        if (!EnableLogs) return false;
        if (ErrorsOnly && !isError) return false;
        return (ActiveCategories & category) != 0;
    }

    private static string FormatMessage(LogCategory category, string message)
    {
        string color = GetCategoryColor(category);
        string categoryName = category.ToString().ToUpper();
        return $"<color={color}>[{categoryName}]</color> {message}";
    }

    private static string GetCategoryColor(LogCategory category)
    {
        return category switch
        {
            LogCategory.Combat => CombatColor,
            LogCategory.Narration => NarrationColor,
            LogCategory.Enviro => EnviroColor,
            LogCategory.Core => CoreColor,
            LogCategory.Audio => AudioColor,
            _ => "#FFFFFF"
        };
    }

    public static void Combat(string message) => Log(LogCategory.Combat, message);
    public static void Narration(string message) => Log(LogCategory.Narration, message);
    public static void Enviro(string message) => Log(LogCategory.Enviro, message);
    public static void Core(string message) => Log(LogCategory.Core, message);
    public static void Audio(string message) => Log(LogCategory.Audio, message);

    public static void DisableCategory(LogCategory category)
    {
        ActiveCategories &= ~category;
    }

    public static void EnableCategory(LogCategory category)
    {
        ActiveCategories |= category;
    }

    public static void SetCategories(LogCategory categories)
    {
        ActiveCategories = categories;
    }
}