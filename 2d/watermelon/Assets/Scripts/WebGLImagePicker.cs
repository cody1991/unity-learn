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
#elif UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select fruit image", "", "png,jpg,jpeg");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        byte[] bytes = System.IO.File.ReadAllBytes(path);
        string payload = tier + "|" + System.Convert.ToBase64String(bytes);

        GameObject target = GameObject.Find(targetObject);
        if (target != null)
        {
            target.SendMessage(callbackMethod, payload);
        }
#else
        Debug.LogWarning("Fruit image upload is only available in WebGL builds.");
#endif
    }
}
