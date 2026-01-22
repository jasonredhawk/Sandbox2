using System.IO;
using UnityEditor;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Builds a prefab with Unity Netcode NetworkManager and a NetworkManagerGO helper.
/// Menu: Tools/Build NetworkManager Prefab
/// </summary>
public static class NetworkManagerPrefabBuilder
{
    private const string prefabPath = "Assets/Prefabs/NetworkManager.prefab";

    [MenuItem("Tools/Build NetworkManager Prefab")]
    public static void Build()
    {
        Directory.CreateDirectory("Assets/Prefabs");

        var go = new GameObject("NetworkManager");
        try
        {
            go.AddComponent<NetworkManager>();
            go.AddComponent<NetworkManagerGO>();

            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            EditorUtility.DisplayDialog("Prefab created", $"Saved to {prefabPath}", "OK");
        }
        finally
        {
            Object.DestroyImmediate(go);
        }
    }
}
