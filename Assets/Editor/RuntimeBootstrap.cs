#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateGameManager()
    {
        if (GameManager.Instance != null || SceneManager.GetActiveScene().name == "MainMenu") return;

        const string prefabPath = "Assets/Prefabs/GameManager.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"GameManager prefab not found:\n{prefabPath}");
            return;
        }
        Object.Instantiate(prefab);
        Debug.LogWarning("GAME NOT LOADED FROM MAIN MENU. SAVING WILL NOT WORK! CHANGING SCENES WILL NOT WORK! LOADED DEFAULT DIFFICULTY: MEDIUM");
    }
}

#endif