using System.Runtime.InteropServices;
using UnityEngine;

public static class WebGLImagePicker
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    static extern void WatermelonPickFruitImage(int tier, string targetObject, string callbackMethod);
#endif

    public static void Open(int tier, string targetObject, string callbackMethod)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WatermelonPickFruitImage(tier, targetObject, callbackMethod);
#else
        Debug.LogWarning("Fruit image upload is only available in WebGL builds.");
#endif
    }
}
