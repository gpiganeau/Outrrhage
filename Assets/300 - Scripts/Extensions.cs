using UnityEngine;
using System.Collections.Generic;

public static class CollectionExtensions
{
    public static T Random<T>(this T[] array)
    {
        if (array.IsEmpty()) return default;
        return array[UnityEngine.Random.Range(0, array.Length)];
    }

    public static T Random<T>(this List<T> list)
    {
        if (list.IsEmpty()) return default;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T[] Shuffle<T>(this T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        return array;
    }

    public static bool IsEmpty<T>(this T[] array) => array == null || array.Length == 0;
    public static bool IsEmpty<T>(this List<T> list) => list == null || list.Count == 0;
}

public static class TransformExtensions
{
    public static void Clear(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            Object.Destroy(child.gameObject);
        }
    }

    public static void ClearImmediate(this Transform transform)
    {
        while (transform.childCount > 0)
        {
            Object.DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public static void ResetLocal(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void SetX(this Transform transform, float x)
    {
        Vector3 pos = transform.position;
        pos.x = x;
        transform.position = pos;
    }

    public static void SetY(this Transform transform, float y)
    {
        Vector3 pos = transform.position;
        pos.y = y;
        transform.position = pos;
    }

    public static void SetZ(this Transform transform, float z)
    {
        Vector3 pos = transform.position;
        pos.z = z;
        transform.position = pos;
    }
}

public static class Vector3Extensions
{
    public static Vector3 Random(float min, float max)
    {
        return new Vector3(
            UnityEngine.Random.Range(min, max),
            UnityEngine.Random.Range(min, max),
            UnityEngine.Random.Range(min, max)
        );
    }

    public static Vector3 RandomXZ(float min, float max)
    {
        return new Vector3(
            UnityEngine.Random.Range(min, max),
            0f,
            UnityEngine.Random.Range(min, max)
        );
    }

    public static Vector3 WithX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);

    public static Vector3 WithY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

    public static Vector3 WithZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

    public static Vector3 Flat(this Vector3 v) => new Vector3(v.x, 0f, v.z);
}

public static class ColorExtensions
{
    public static Color Random()
    {
        return new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
        );
    }

    public static Color SetAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);
}

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static void SetLayerRecursive(this GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            child.gameObject.SetLayerRecursive(layer);
        }
    }
}

public static class AnimatorExtensions
{
    public static float GetClipLength(this Animator animator, string clipName)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Contains(clipName))
                return clip.length;
        }
        return 0f;
    }
}
