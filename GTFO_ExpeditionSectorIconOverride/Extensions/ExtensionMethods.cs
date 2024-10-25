using System.Collections.Generic;
using UnityEngine;

namespace ExSeIcOv.Extensions;

internal static class ExtensionMethods
{
    public static IEnumerable<Transform> Children(this Transform self)
    {
        for (int i = 0; i < self.childCount; i++)
        {
            yield return self.GetChild(i);
        }
    }

    public static T GetOrAddComponent<T>(this GameObject self) where T : Component
    {
        var comp = self.GetComponent<T>();

        if(comp == null)
        {
            comp = self.AddComponent<T>();
        }

        return comp;
    }

    public static void DontDestroyAndSetHideFlags(this UnityEngine.Object obj)
    {
        UnityEngine.Object.DontDestroyOnLoad(obj);
        obj.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
    }
}
